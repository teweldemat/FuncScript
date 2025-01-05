import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
import { SERVER_URL, SERVER_WS_URL } from '../backend';
import { cpSync } from 'fs';

export enum ExpressionType {
    ClearText = 'ClearText',
    FuncScript = 'FuncScript',
    FuncScriptTextTemplate = 'FuncScriptTextTemplate',
    FsStudioParentNode = 'FsStudioParentNode',
}

export interface NodeState {
    name: string;
    expressionType: ExpressionType;
    childrenCount: number;
    expression: string | null;
    dataLoaded: boolean;
    evaluating: boolean;
    cachedValue: string | null;
    isCached: boolean;
    evaluationRes: any;
    evaluationError: string | null;
    childNodes: NodeState[] | null;
}

export interface SessionState {
    sessionId: string;
    filePath: string;
    nodes: NodeState[];
    evaluationInProgressNodePath?: string;
    expandedNodes: Record<string, boolean>;
    messages: string[];
    markdown: string;
}

interface DirectoryListResult {
    directories: string[];
    files: string[];
}

interface SessionsContextValue {
    sessions: Record<string, SessionState>;
    createSession: (filePath: string) => Promise<SessionState>;
    unloadSession: (session: SessionState) => Promise<void>;
    getEvaluationStatus: (session: SessionState) => Promise<void>;
    loadNode: (session: SessionState, nodePath: string) => Promise<NodeState>;
    evaluateNode: (session: SessionState, nodePath: string) => Promise<void>;
    createNode: (
        session: SessionState,
        parentNodePath: string | null,
        name: string,
        expression: string,
        expressionType: ExpressionType
    ) => Promise<void>;
    removeNode: (session: SessionState, nodePath: string) => Promise<void>;
    renameNode: (session: SessionState, nodePath: string, newName: string) => Promise<void>;
    changeExpressionType: (
        session: SessionState,
        nodePath: string,
        expressionType: ExpressionType
    ) => Promise<void>;
    updateExpression: (
        session: SessionState,
        nodePath: string,
        expression: string
    ) => Promise<void>;
    moveNode: (
        session: SessionState,
        nodePath: string,
        newParentPath: string | null
    ) => Promise<void>;
    listDirectory: (path: string) => Promise<DirectoryListResult>;
    loadChildNodeList: (session: SessionState, nodePath: string | null) => Promise<NodeState[]>;
    toggleNodeExpanded: (session: SessionState, nodePath: string) => void;
    clearSessionLog: (session: SessionState) => void;
    createFolder: (path: string, name: string) => Promise<void>;
    createFile: (path: string, name: string) => Promise<void>;
    duplicateFile: (path: string, name: string) => Promise<void>;
    deleteItem: (path: string) => Promise<void>;
    renameItem: (path: string, newName: string) => Promise<void>;
    setRootFolder: (newRootFolder: string) => Promise<void>;
}

const ExecutionSessionContext = createContext<SessionsContextValue | null>(null);

export function findNodeByPath(rootNodes: NodeState[], nodePath: string): NodeState | null {
    const parts = nodePath.split('.');
    let currentNodes = rootNodes;
    let currentNode: NodeState | null = null;

    for (const part of parts) {
        currentNode = currentNodes.find((n) => n.name === part) ?? null;
        if (!currentNode) return null;
        if (!currentNode.childNodes) currentNode.childNodes = [];
        currentNodes = currentNode.childNodes;
    }
    return currentNode;
}

function updateSessionNode(rootNodes: NodeState[], nodePath: string, newNode: NodeState): NodeState[] {
    const parts = nodePath.split('.');
    const clonedRootNodes = [...rootNodes];

    let currentNodes = clonedRootNodes;
    for (let i = 0; i < parts.length; i++) {
        const part = parts[i];
        const index = currentNodes.findIndex((n) => n.name === part);
        if (index < 0) {
            return clonedRootNodes;
        }
        if (i === parts.length - 1) {
            currentNodes[index] = newNode;
        } else {
            const childNode = { ...currentNodes[index] };
            if (!childNode.childNodes) childNode.childNodes = [];
            childNode.childNodes = [...childNode.childNodes];
            currentNodes[index] = childNode;
            currentNodes = childNode.childNodes;
        }
    }

    return clonedRootNodes;
}

export function ExecutionSessionProvider({ children }: { children: React.ReactNode }) {
    const [sessions, setSessions] = useState<Record<string, SessionState>>({});
    const wsRef = useRef<WebSocket | null>(null);

    useEffect(() => {
        const ws = new WebSocket(SERVER_WS_URL);
        wsRef.current = ws;
        ws.onmessage = (evt) => handleWsMessage(evt);
        return () => {
            ws.close();
        };
    }, []);

    const handleWsMessage = (evt: MessageEvent) => {
        const data = JSON.parse(evt.data);
        if (data.cmd === 'evaluation_success' || data.cmd === 'evaluation_error') {
            const { sessionId } = data.data;
            setSessions((prev) => {
                const session = prev[sessionId];
                if (!session) return prev;

                const nodePath = session.evaluationInProgressNodePath;
                if (!nodePath) return prev;

                const node = findNodeByPath(session.nodes, nodePath);
                if (!node) return prev;

                const newNode = { ...node, evaluating: false };
                if (data.cmd === 'evaluation_success') {
                    newNode.evaluationRes = data.data.result;
                    newNode.evaluationError = null;
                } else {
                    newNode.evaluationError = data.data.error;
                    newNode.evaluationRes = null;
                }

                const updatedNodes = updateSessionNode(session.nodes, nodePath, newNode);

                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                        evaluationInProgressNodePath: undefined,
                        nodes: updatedNodes,
                    },
                };
            });
        } else if (data.cmd === 'log') {
            const { sessionId, message } = data.data;
            if (!sessionId) return;
            setSessions((prev) => {
                const session = prev[sessionId];
                if (!session) return prev;
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                        messages: [...session.messages, message],
                    },
                };
            });
        } else if (data.cmd === 'clear') {
            const { sessionId } = data.data;
            if (!sessionId) return;
            setSessions((prev) => {
                const session = prev[sessionId];
                if (!session) return prev;
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                        messages: [],
                    },
                };
            });
        } else if (data.cmd === 'markdown') {
            const { sessionId, markdown: md } = data.data;
            if (!sessionId) return;
            setSessions((prev) => {
                const session = prev[sessionId];
                if (!session) return prev;
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                        markdown: md,
                    },
                };
            });
        }
    };

    const createSession = async (filePath: string) => {
        const existingSession = Object.values(sessions).find(
            (session) => session.filePath === filePath
        );
        if (existingSession) {
            return existingSession;
        }
        const res = await fetch(`${SERVER_URL}/api/sessions/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fromFile: filePath }),
        });
        if (!res.ok) throw new Error(await res.text());

        const { sessionId } = await res.json();
        const children = await fetchChildren(sessionId);

        const newSession: SessionState = {
            sessionId,
            filePath,
            nodes: children,
            expandedNodes: {},
            messages: [],
            markdown: '',
        };

        setSessions((prev) => ({
            ...prev,
            [sessionId]: newSession,
        }));

        return newSession;
    };

    const unloadSession = async (session: SessionState) => {
        const res = await fetch(`${SERVER_URL}/api/sessions/unload?sessionId=${session.sessionId}`, {
            method: 'POST'
        });
        if (!res.ok) throw new Error(await res.text());
        setSessions((prev) => {
            const next = { ...prev };
            delete next[session.sessionId];
            return next;
        });
    };

    const getEvaluationStatus = async (session: SessionState) => {
        const res = await fetch(`${SERVER_URL}/api/sessions/${session.sessionId}/node/evaluate/status`);
        if (!res.ok) throw new Error(await res.text());
        const data = await res.json();

        setSessions((prev) => {
            const current = prev[session.sessionId];
            if (!current) return prev;

            if (data.status === 'idle') {
                return prev;
            } else if (data.status === 'inprogress') {
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                    },
                };
            } else if (data.status === 'error') {
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                    },
                };
            } else if (data.status === 'success') {
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                    },
                };
            }
            return prev;
        });
    };

    const loadNode = async (session: SessionState, nodePath: string) => {
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node?${params.toString()}`
        );
        if (!res.ok) throw new Error(await res.text());

        const nodeInfo = await res.json();
        const newNode: NodeState = {
            name: nodeInfo.name,
            expressionType: nodeInfo.expressionType,
            childrenCount: nodeInfo.childrenCount,
            expression: nodeInfo.expression,
            dataLoaded: true,
            evaluating: false,
            evaluationRes: null,
            evaluationError: null,
            cachedValue: null,
            isCached: false,
            childNodes: [],
        };

        setSessions((prev) => {
            const current = prev[session.sessionId];
            if (!current) return prev;

            const updatedNodes = updateSessionNode(current.nodes, nodePath, newNode);
            return {
                ...prev,
                [session.sessionId]: {
                    ...current,
                    nodes: updatedNodes,
                },
            };
        });

        return newNode;
    };

    const evaluateNode = async (session: SessionState, nodePath: string) => {
        setSessions((prev) => {
            const current = prev[session.sessionId];
            if (!current) return prev;

            const node = findNodeByPath(current.nodes, nodePath);
            if (!node) return prev;

            const newNode = {
                ...node,
                evaluating: true,
                evaluationError: null,
                evaluationRes: null,
            };

            const updatedNodes = updateSessionNode(current.nodes, nodePath, newNode);

            return {
                ...prev,
                [session.sessionId]: {
                    ...current,
                    evaluationInProgressNodePath: nodePath,
                    nodes: updatedNodes,
                },
            };
        });

        const params = new URLSearchParams({ nodePath });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node/evaluate?${params.toString()}`,
            { method: 'POST' }
        );
        if (!res.ok) throw new Error(await res.text());
    };

    const createNode = async (
        session: SessionState,
        parentNodePath: string | null,
        name: string,
        expression: string,
        expressionType: ExpressionType
    ) => {
        const body = {
            parentNodePath,
            name,
            expression,
            expressionType,
        };
        const res = await fetch(`${SERVER_URL}/api/sessions/${session.sessionId}/node`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        await loadChildNodeList(session, parentNodePath);
    };

    const removeNode = async (session: SessionState, nodePath: string) => {
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node?${params.toString()}`,
            {
                method: 'DELETE'
            }
        );
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const parentNodePath = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        await loadChildNodeList(session, parentNodePath);
    };

    const renameNode = async (session: SessionState, nodePath: string, newName: string) => {
        const body = { nodePath, newName };
        const res = await fetch(`${SERVER_URL}/api/sessions/${session.sessionId}/node/rename`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const parentNodePath = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        await loadChildNodeList(session, parentNodePath);
    };

    const changeExpressionType = async (
        session: SessionState,
        nodePath: string,
        expressionType: ExpressionType
    ) => {
        const params = new URLSearchParams({ nodePath, expressionType });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node/expressionType?${params.toString()}`,
            {
                method: 'POST'
            }
        );
        if (!res.ok) throw new Error(await res.text());

        await loadNode(session, nodePath);
    };

    const updateExpression = async (
        session: SessionState,
        nodePath: string,
        expression: string
    ) => {
        const model = { expression };
        const urlPath = encodeURIComponent(nodePath);
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node/expression/${urlPath}`,
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(model),
            }
        );
        if (!res.ok) throw new Error(await res.text());

        await loadNode(session, nodePath);
    };

    const moveNode = async (
        session: SessionState,
        nodePath: string,
        newParentPath: string | null
    ) => {
        const body = { nodePath, newParentPath };
        const res = await fetch(`${SERVER_URL}/api/sessions/${session.sessionId}/node/move`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const oldParent = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        if (oldParent) await loadChildNodeList(session, oldParent);
        await loadChildNodeList(session, newParentPath ?? null);
    };

    const listDirectory = async (path: string) => {
        const url = `${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles?path=${encodeURIComponent(
            path
        )}`;
        const res = await fetch(url);
        if (!res.ok) throw new Error(await res.text());
        const result = (await res.json()) as DirectoryListResult;
        return result;
    };

    const fetchChildren = async (sessionId: string): Promise<NodeState[]> => {
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/children`);
        if (!res.ok) {
            throw new Error(await res.text());
        }
        const childNodesRaw: any[] = await res.json();

        return childNodesRaw.map((cn: any): NodeState => {
            return {
                name: cn.name,
                expressionType: cn.expressionType ?? null,
                childrenCount: cn.childrenCount ?? 0,
                expression: null,
                dataLoaded: false,
                evaluating: false,
                evaluationRes: null,
                evaluationError: null,
                cachedValue: null,
                isCached: false,
                childNodes: null,
            };
        });
    };

    const loadChildNodeList = async (session: SessionState, nodePath: string | null) => {
        const params = new URLSearchParams();
        if (nodePath) {
            params.set('nodePath', nodePath);
        }
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${session.sessionId}/node/children?${params.toString()}`
        );
        if (!res.ok) throw new Error(await res.text());
        const childNodesRaw = await res.json();

        setSessions((prev) => {
            let updatedSession = prev[session.sessionId];
            if (!updatedSession) return prev;

            const children: NodeState[] = childNodesRaw.map((cn: any) => {
                return {
                    name: cn.name,
                    expressionType: cn.expressionType,
                    childrenCount: cn.childrenCount,
                    expression: null,
                    dataLoaded: false,
                    evaluating: false,
                    evaluationRes: null,
                    evaluationError: null,
                    cachedValue: null,
                    isCached: false,
                    childNodes: null,
                };
            });

            if (nodePath) {
                const parentNode = findNodeByPath(updatedSession.nodes, nodePath);
                if (parentNode) {
                    const newParentNode: NodeState = {
                        ...parentNode,
                        childNodes: children,
                    };
                    const updatedNodes = updateSessionNode(updatedSession.nodes, nodePath, newParentNode);
                    updatedSession = { ...updatedSession, nodes: updatedNodes };
                }
            } else {
                updatedSession = { ...updatedSession, nodes: children };
            }

            return {
                ...prev,
                [session.sessionId]: updatedSession,
            };
        });

        return nodePath
            ? childNodesRaw.map((cn: any) => {
                  const fullPath = nodePath ? `${nodePath}.${cn.name}` : cn.name;
                  return findNodeByPath(sessions[session.sessionId].nodes, fullPath)!;
              })
            : childNodesRaw.map((cn: any) => findNodeByPath(sessions[session.sessionId].nodes, cn.name)!);
    };

    const toggleNodeExpanded = (session: SessionState, nodePath: string) => {
        setSessions((prev) => {
            const current = prev[session.sessionId];
            if (!current) return prev;
            console.log('tog: '+nodePath)
            console.log(current.expandedNodes)
            console.log(current.expandedNodes[nodePath])
            const isOpen = !!current.expandedNodes[nodePath];
            console.log('new expand state: :'+isOpen)
            return {
                ...prev,
                [session.sessionId]: {
                    ...current,
                    expandedNodes: {
                        ...current.expandedNodes,
                        [nodePath]: !isOpen,
                    },
                },
            };
        });
    };

    const clearSessionLog = (session: SessionState) => {
        setSessions((prev) => {
            const current = prev[session.sessionId];
            if (!current) return prev;
            return {
                ...prev,
                [session.sessionId]: {
                    ...current,
                    messages: [],
                },
            };
        });
    };

    const createFolder = async (path: string, name: string) => {
        const res = await fetch(`${SERVER_URL}/api/FileSystem/CreateFolder`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ path, name }),
        });
        if (!res.ok) throw new Error(await res.text());
    };

    const createFile = async (path: string, name: string) => {
        const res = await fetch(`${SERVER_URL}/api/FileSystem/CreateFile`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ path, name }),
        });
        if (!res.ok) throw new Error(await res.text());
    };

    const duplicateFile = async (path: string, name: string) => {
        const res = await fetch(`${SERVER_URL}/api/FileSystem/DuplicateFile`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ path, name }),
        });
        if (!res.ok) throw new Error(await res.text());
    };

    const renameItem = async (path: string, newName: string) => {
        const res = await fetch(`${SERVER_URL}/api/FileSystem/RenameItem`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ path, name: newName }),
        });
        if (!res.ok) throw new Error(await res.text());
    
        const segments = path.split('/');
        segments[segments.length - 1] = newName;
        const newPath = segments.join('/');
    
        setSessions((prev) => {
            const next = { ...prev };
            Object.keys(next).forEach((sessionId) => {
                const s = next[sessionId];
                if (s.filePath === path) {
                    s.filePath = newPath;
                } else if (s.filePath.startsWith(path + '/')) {
                    s.filePath = s.filePath.replace(path, newPath);
                }
            });
            return next;
        });
    };
    
    const deleteItem = async (path: string) => {
        const url = `${SERVER_URL}/api/FileSystem/DeleteItem?path=${encodeURIComponent(path)}`;
        const res = await fetch(url, { method: 'DELETE' });
        if (!res.ok) throw new Error(await res.text());
    
        setSessions((prev) => {
            const next = { ...prev };
            Object.keys(next).forEach((sessionId) => {
                const s = next[sessionId];
                if (s.filePath === path || s.filePath.startsWith(path + '/')) {
                    delete next[sessionId];
                }
            });
            return next;
        });
    };

    const setRootFolder = async (newRootFolder: string) => {
        const res = await fetch(`${SERVER_URL}/api/FileSystem/SetRootFolder`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ newRootFolder }),
        });
        if (!res.ok) throw new Error(await res.text());
        setSessions({});
    };

    return (
        <ExecutionSessionContext.Provider
            value={{
                sessions,
                createSession,
                unloadSession,
                getEvaluationStatus,
                loadNode,
                evaluateNode,
                createNode,
                removeNode,
                renameNode,
                changeExpressionType,
                updateExpression,
                moveNode,
                listDirectory,
                loadChildNodeList,
                toggleNodeExpanded,
                clearSessionLog,
                createFolder,
                createFile,
                duplicateFile,
                deleteItem,
                renameItem,
                setRootFolder,
            }}
        >
            {children}
        </ExecutionSessionContext.Provider>
    );
}

export function useExecutionSession() {
    return useContext(ExecutionSessionContext);
}

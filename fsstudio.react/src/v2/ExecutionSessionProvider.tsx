// ExecutionSessionProvider.tsx
import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
import { SERVER_URL, SERVER_WS_URL } from '../backend';

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
    createSession: (filePath: string) => Promise<string>;
    unloadSession: (sessionId: string) => Promise<void>;
    getEvaluationStatus: (sessionId: string) => Promise<void>;
    loadNode: (sessionId: string, nodePath: string) => Promise<NodeState>;
    evaluateNode: (sessionId: string, nodePath: string) => Promise<void>;
    createNode: (
        sessionId: string,
        parentNodePath: string | null,
        name: string,
        expression: string,
        expressionType: ExpressionType
    ) => Promise<void>;
    removeNode: (sessionId: string, nodePath: string) => Promise<void>;
    renameNode: (sessionId: string, nodePath: string, newName: string) => Promise<void>;
    changeExpressionType: (
        sessionId: string,
        nodePath: string,
        expressionType: ExpressionType
    ) => Promise<void>;
    updateExpression: (
        sessionId: string,
        nodePath: string,
        expression: string
    ) => Promise<void>;
    moveNode: (
        sessionId: string,
        nodePath: string,
        newParentPath: string | null
    ) => Promise<void>;
    listDirectory: (path: string) => Promise<DirectoryListResult>;
    loadChildNodeList: (sessionId: string, nodePath: string | null) => Promise<NodeState[]>;
    toggleNodeExpanded: (sessionId: string, nodePath: string) => void;
    clearSessionLog: (sessionId: string) => void;
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
            return existingSession.sessionId;
        }
        const res = await fetch(`${SERVER_URL}/api/sessions/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fromFile: filePath }),
        });
        if (!res.ok) throw new Error(await res.text());

        const { sessionId } = await res.json();
        const children = await fetchChildren(sessionId);

        setSessions((prev) => ({
            ...prev,
            [sessionId]: {
                sessionId,
                filePath,
                nodes: children,
                expandedNodes: {},
                messages: [],
                markdown: '',
            },
        }));

        return sessionId;
    };

    const unloadSession = async (sessionId: string) => {
        const res = await fetch(`${SERVER_URL}/api/sessions/unload?sessionId=${sessionId}`, {
            method: 'POST'
        });
        if (!res.ok) throw new Error(await res.text());
        setSessions((prev) => {
            const next = { ...prev };
            delete next[sessionId];
            return next;
        });
    };

    const getEvaluationStatus = async (sessionId: string) => {
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/evaluate/status`);
        if (!res.ok) throw new Error(await res.text());
        const data = await res.json();

        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;

            if (data.status === 'idle') {
                return prev;
            } else if (data.status === 'inprogress') {
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                    },
                };
            } else if (data.status === 'error') {
                // Could attach the error to a known node path if needed
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                    },
                };
            } else if (data.status === 'success') {
                // Could attach the result to a known node path if needed
                return {
                    ...prev,
                    [sessionId]: {
                        ...session,
                    },
                };
            }
            return prev;
        });
    };

    const loadNode = async (sessionId: string, nodePath: string) => {
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node?${params.toString()}`);
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
            const session = prev[sessionId];
            if (!session) return prev;

            const updatedNodes = updateSessionNode(session.nodes, nodePath, newNode);
            return {
                ...prev,
                [sessionId]: {
                    ...session,
                    nodes: updatedNodes,
                },
            };
        });

        return newNode;
    };

    const evaluateNode = async (sessionId: string, nodePath: string) => {
        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;

            const node = findNodeByPath(session.nodes, nodePath);
            if (!node) return prev;

            const newNode = {
                ...node,
                evaluating: true,
                evaluationError: null,
                evaluationRes: null,
            };

            const updatedNodes = updateSessionNode(session.nodes, nodePath, newNode);

            return {
                ...prev,
                [sessionId]: {
                    ...session,
                    evaluationInProgressNodePath: nodePath,
                    nodes: updatedNodes,
                },
            };
        });

        const params = new URLSearchParams({ nodePath });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${sessionId}/node/evaluate?${params.toString()}`,
            { method: 'POST' }
        );
        if (!res.ok) throw new Error(await res.text());
    };

    const createNode = async (
        sessionId: string,
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
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        await loadChildNodeList(sessionId, parentNodePath);
    };

    const removeNode = async (sessionId: string, nodePath: string) => {
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${sessionId}/node?${params.toString()}`,
            {
                method: 'DELETE'
            }
        );
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const parentNodePath = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        await loadChildNodeList(sessionId, parentNodePath);
    };

    const renameNode = async (sessionId: string, nodePath: string, newName: string) => {
        const body = { nodePath, newName };
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/rename`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const parentNodePath = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        await loadChildNodeList(sessionId, parentNodePath);
    };

    const changeExpressionType = async (
        sessionId: string,
        nodePath: string,
        expressionType: ExpressionType
    ) => {
        const params = new URLSearchParams({ nodePath, expressionType });
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${sessionId}/node/expressionType?${params.toString()}`,
            {
                method: 'POST'
            }
        );
        if (!res.ok) throw new Error(await res.text());

        await loadNode(sessionId, nodePath);
    };

    const updateExpression = async (
        sessionId: string,
        nodePath: string,
        expression: string
    ) => {
        const model = { expression };
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/expression/${encodeURIComponent(nodePath)}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(model),
        });
        if (!res.ok) throw new Error(await res.text());

        await loadNode(sessionId, nodePath);
    };

    const moveNode = async (
        sessionId: string,
        nodePath: string,
        newParentPath: string | null
    ) => {
        const body = { nodePath, newParentPath };
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/move`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body),
        });
        if (!res.ok) throw new Error(await res.text());

        const parentPathParts = nodePath.split('.');
        parentPathParts.pop();
        const oldParent = parentPathParts.length > 0 ? parentPathParts.join('.') : null;
        if (oldParent) await loadChildNodeList(sessionId, oldParent);
        await loadChildNodeList(sessionId, newParentPath ?? null);
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

    const loadChildNodeList = async (sessionId: string, nodePath: string | null) => {
        const session = sessions[sessionId];
        if (!session) {
            throw new Error('Session not found');
        }
        const params = new URLSearchParams();
        if (nodePath) {
            params.set('nodePath', nodePath);
        }
        const res = await fetch(
            `${SERVER_URL}/api/sessions/${sessionId}/node/children?${params.toString()}`
        );
        if (!res.ok) throw new Error(await res.text());
        const childNodesRaw = await res.json();

        setSessions((prev) => {
            let updatedSession = prev[sessionId];
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
                [sessionId]: updatedSession,
            };
        });

        return nodePath
            ? childNodesRaw.map((cn: any) => {
                  const fullPath = nodePath ? `${nodePath}.${cn.name}` : cn.name;
                  return findNodeByPath(sessions[sessionId].nodes, fullPath)!;
              })
            : childNodesRaw.map((cn: any) => findNodeByPath(sessions[sessionId].nodes, cn.name)!);
    };

    const toggleNodeExpanded = (sessionId: string, nodePath: string) => {
        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;
            const isOpen = !!session.expandedNodes[nodePath];
            return {
                ...prev,
                [sessionId]: {
                    ...session,
                    expandedNodes: {
                        ...session.expandedNodes,
                        [nodePath]: !isOpen,
                    },
                },
            };
        });
    };

    const clearSessionLog = (sessionId: string) => {
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
                const session = next[sessionId];
                if (session.filePath === path) {
                    session.filePath = newPath;
                } else if (session.filePath.startsWith(path + '/')) {
                    session.filePath = session.filePath.replace(path, newPath);
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
                const session = next[sessionId];
                if (session.filePath === path || session.filePath.startsWith(path + '/')) {
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

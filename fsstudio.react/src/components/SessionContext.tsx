    import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
    import { SERVER_URL, SERVER_WS_URL } from '../backend';
    import {
        findNodeByPath,
        updateSessionNode,
        listDirectory,
        fetchChildren,
        createFolder,
        createFile,
        duplicateFile
    } from './SessionUtils';

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
        rootNode: NodeState;
        expandedNodes: Record<string, boolean>;
        messages: string[];
        markdown: string;
        evaluationInProgressNodePath?: string;
        selectedNodePath: string | null;
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
        setSelectedNodePath: (session: SessionState, nodePath: string | null) => void;
        saveExpression: (
            session: SessionState,
            nodePath: string,
            newExpression: string,
            thenEvaluate: boolean
        ) => Promise<void>;
    }

    const ExecutionSessionContext = createContext<SessionsContextValue | null>(null);

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
                    const node = findNodeByPath(session, nodePath);
                    if (!node) return prev;
                    const newNode = { ...node, evaluating: false };
                    if (data.cmd === 'evaluation_success') {
                        newNode.evaluationRes = data.data.result;
                        newNode.evaluationError = null;
                    } else {
                        newNode.evaluationError = data.data.error;
                        newNode.evaluationRes = null;
                    }
                    const updatedRoot = updateSessionNode(session.rootNode, nodePath, newNode);
                    return {
                        ...prev,
                        [sessionId]: {
                            ...session,
                            evaluationInProgressNodePath: undefined,
                            rootNode: updatedRoot,
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
            const rootNode: NodeState = {
                name: '',
                expressionType: ExpressionType.FsStudioParentNode,
                childrenCount: children.length,
                expression: null,
                dataLoaded: true,
                evaluating: false,
                evaluationRes: null,
                evaluationError: null,
                cachedValue: null,
                isCached: false,
                childNodes: children,
            };
            const newSession: SessionState = {
                sessionId,
                filePath,
                rootNode,
                expandedNodes: {},
                messages: [],
                markdown: '',
                selectedNodePath: null,
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
                    return { ...prev };
                } else if (data.status === 'error') {
                    return { ...prev };
                } else if (data.status === 'success') {
                    return { ...prev };
                }
                return prev;
            });
        };

        const setSelectedNodePath = (session: SessionState, nodePath: string | null) => {
            setSessions((prev) => {
                const current = prev[session.sessionId];
                if (!current) return prev;
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                        selectedNodePath: nodePath,
                    },
                };
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
                const updatedRoot = updateSessionNode(current.rootNode, nodePath, newNode);
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                        rootNode: updatedRoot,
                    },
                };
            });
            return newNode;
        };

        const evaluateNode = async (session: SessionState, nodePath: string) => {
            setSessions((prev) => {
                const current = prev[session.sessionId];
                if (!current) return prev;
                const node = findNodeByPath(current, nodePath);
                if (!node) return prev;
                const newNode = {
                    ...node,
                    evaluating: true,
                    evaluationError: null,
                    evaluationRes: null,
                };
                const updatedRoot = updateSessionNode(current.rootNode, nodePath, newNode);
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                        evaluationInProgressNodePath: nodePath,
                        rootNode: updatedRoot,
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
            if(parentNodePath && parentNodePath!='')
                await loadNode(session,parentNodePath)
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
                    const parentNode = findNodeByPath(updatedSession, nodePath);
                    if (parentNode) {
                        const newParentNode: NodeState = {
                            ...parentNode,
                            childNodes: children,
                        };
                        const updatedRoot = updateSessionNode(
                            updatedSession.rootNode,
                            nodePath,
                            newParentNode
                        );
                        updatedSession = { ...updatedSession, rootNode: updatedRoot };
                    }
                } else {
                    updatedSession = {
                        ...updatedSession,
                        rootNode: {
                            ...updatedSession.rootNode,
                            childNodes: children,
                            childrenCount: children.length,
                        },
                    };
                }
                return {
                    ...prev,
                    [session.sessionId]: updatedSession,
                };
            });
            return nodePath
                ? childNodesRaw.map((cn: any) => {
                    const fullPath = nodePath ? `${nodePath}.${cn.name}` : cn.name;
                    return findNodeByPath(sessions[session.sessionId], fullPath)!;
                })
                : childNodesRaw.map(
                    (cn: any) => findNodeByPath(sessions[session.sessionId], cn.name)!
                );
        };

        const toggleNodeExpanded = (session: SessionState, nodePath: string) => {
            setSessions((prev) => {
                const current = prev[session.sessionId];
                if (!current) return prev;
                const isOpen = !!current.expandedNodes[nodePath];
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

        const saveExpression = async (
            session: SessionState,
            nodePath: string,
            newExpression: string,
            thenEvaluate: boolean
        ) => {
            const urlPath = encodeURIComponent(nodePath);
            const res = await fetch(
                `${SERVER_URL}/api/sessions/${session.sessionId}/node/expression/${urlPath}`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ expression: newExpression }),
                }
            );
            if (!res.ok) throw new Error(await res.text());

            setSessions((prev) => {
                const current = prev[session.sessionId];
                if (!current) return prev;
                const node = findNodeByPath(current, nodePath);
                if (!node) return prev;
                const updatedNode = { ...node, expression: newExpression };
                const updatedRoot = updateSessionNode(current.rootNode, nodePath, updatedNode);
                return {
                    ...prev,
                    [session.sessionId]: {
                        ...current,
                        rootNode: updatedRoot,
                    },
                };
            });

            if (thenEvaluate) {
                await evaluateNode(session, nodePath);
            }
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
                    listDirectory: (path: string) => listDirectory(path),
                    loadChildNodeList,
                    toggleNodeExpanded,
                    clearSessionLog,
                    createFolder: (path: string, name: string) => createFolder(path, name),
                    createFile: (path: string, name: string) => createFile(path, name),
                    duplicateFile: (path: string, name: string) => duplicateFile(path, name),
                    deleteItem,
                    renameItem,
                    setRootFolder,
                    setSelectedNodePath,
                    saveExpression,
                }}
            >
                {children}
            </ExecutionSessionContext.Provider>
        );
    }

    export function useExecutionSession() {
        return useContext(ExecutionSessionContext);
    }
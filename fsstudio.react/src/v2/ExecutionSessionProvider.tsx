import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
import { SERVER_URL, SERVER_WS_URL } from '../backend';

interface ExpressionNodeInfo {
    name: string;
    expressionType: number;
    childrenCount: number;
}
interface ExpressionNodeInfoWithExpression {
    name: string;
    expressionType: number;
    childrenCount: number;
    expression: string | null;
}

interface NodeState extends ExpressionNodeInfoWithExpression {
    evaluating: boolean;
    cachedValue:string|null;
    isCached:boolean;
    evaluationRes: any;
    evaluationError: string | null;
    childNodes?: ExpressionNodeInfo[];
}

interface SessionState {
    sessionId: string;
    filePath: string;
    nodes: Record<string, NodeState>;
    evaluationInProgressNodePath?: string;
}

interface DirectoryListResult {
    directories: string[];
    files: string[];
}

interface SessionsContextValue {
    sessions: Record<string, SessionState>;
    createSession: (filePath: string) => Promise<string>;
    loadNode: (sessionId: string, nodePath: string) => Promise<ExpressionNodeInfoWithExpression>;
    evaluateNode: (sessionId: string, nodePath: string) => Promise<void>;
    listDirectory: (path: string) => Promise<DirectoryListResult>;
    loadChildNodeList: (sessionId: string,nodePath: string) => Promise<ExpressionNodeInfo[]>;
}

const ExecutionSessionContext = createContext<SessionsContextValue | null>(null);

export function ExecutionSessionProvider({ children }: { children: React.ReactNode }) {
    const [sessions, setSessions] = useState<Record<string, SessionState>>({});
    const wsRef = useRef<WebSocket | null>(null);

    useEffect(() => {
        const ws = new WebSocket(`${SERVER_WS_URL}/ws`);
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
                const node = session.nodes[nodePath];
                if (!node) return prev;
                const newNode = { ...node, evaluating: false };

                if (data.cmd === 'evaluation_success') {
                    newNode.evaluationRes = data.data.result;
                    newNode.evaluationError = null;
                } else {
                    newNode.evaluationError = data.data.error;
                    newNode.evaluationRes = null;
                }

                const newSession = {
                    ...session,
                    evaluationInProgressNodePath: undefined,
                    nodes: {
                        ...session.nodes,
                        [nodePath]: newNode,
                    },
                };
                return { ...prev, [sessionId]: newSession };
            });
        }
    };

    const createSession = async (filePath: string) => {
        const existingSession = Object.values(sessions).find(
            (session) => session.filePath === filePath
        );
        if (existingSession) {
            throw new Error(`File "${filePath}" is already loaded`);
        }
        const res = await fetch(`${SERVER_URL}/api/sessions/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fromFile: filePath }),
        });
        if (!res.ok) throw new Error(await res.text());
        const { sessionId } = await res.json();
        setSessions((prev) => {
            return {
                ...prev,
                [sessionId]: {
                    sessionId,
                    filePath,
                    nodes: {},
                },
            };
        });
        return sessionId;
    };

    const loadNode = async (sessionId: string, nodePath: string) => {
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node?${params.toString()}`);
        if (!res.ok) throw new Error(await res.text());
        const nodeInfo = (await res.json()) as ExpressionNodeInfoWithExpression;

        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;
            const newNode: NodeState = {
                ...nodeInfo,
                evaluating: false,
                evaluationRes: null,
                evaluationError: null,
                cachedValue:null,
                isCached:false,
            };
            const newSession = {
                ...session,
                nodes: {
                    ...session.nodes,
                    [nodePath]: newNode,
                },
            };
            return { ...prev, [sessionId]: newSession };
        });
        return nodeInfo;
    };

    const evaluateNode = async (sessionId: string, nodePath: string) => {
        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;
            const node = session.nodes[nodePath];
            if (!node) return prev;
            const newNode = { ...node, evaluating: true, evaluationError: null, evaluationRes: null };
            const newSession = {
                ...session,
                evaluationInProgressNodePath: nodePath,
                nodes: {
                    ...session.nodes,
                    [nodePath]: newNode,
                },
            };
            return { ...prev, [sessionId]: newSession };
        });
        const params = new URLSearchParams({ nodePath });
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/evaluate?${params.toString()}`, {
            method: 'POST',
        });
        if (!res.ok) throw new Error(await res.text());
    };

    const listDirectory = async (path: string) => {
        const url = `${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles?path=${encodeURIComponent(path)}`;
        const res = await fetch(url);
        if (!res.ok) throw new Error(await res.text());
        return (await res.json()) as DirectoryListResult;
    };

    const loadChildNodeList = async (sessionId: string, nodePath: string | null) => {
        const params = new URLSearchParams();
        if (nodePath) {
            params.set('nodePath', nodePath);
        }
        const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/children?${params.toString()}`);
        if (!res.ok) throw new Error(await res.text());
        const childNodes = (await res.json()) as ExpressionNodeInfo[];
    
        setSessions((prev) => {
            const session = prev[sessionId];
            if (!session) return prev;
            const newSession = { ...session };
    
            if (nodePath) {
                const node = newSession.nodes[nodePath];
                if (node) {
                    newSession.nodes[nodePath] = {
                        ...node,
                        childNodes
                    };
                }
            } else {
                (newSession as any).rootChildren = childNodes;
            }
            return { ...prev, [sessionId]: newSession };
        });
        return childNodes;
    };

    return (
        <ExecutionSessionContext.Provider
            value={{
                sessions,
                createSession,
                loadNode,
                evaluateNode,
                listDirectory,
                loadChildNodeList
            }}
        >
            {children}
        </ExecutionSessionContext.Provider>
    );
}

export function useExecutionSession() {
    return useContext(ExecutionSessionContext);
}

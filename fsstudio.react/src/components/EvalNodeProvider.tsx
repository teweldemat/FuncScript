import React, { createContext, useContext, useEffect, useState } from 'react';
import axios from 'axios';
import { SERVER_URL } from '../backend';

export interface NodeItem {
    name: string;
    expressionType: ExpressionType;
    childrenCount: number;
    path: string|null;
    expression: string | null;
}

export enum ExpressionType {
    ClearText = 'ClearText',
    FuncScript = 'FuncScript',
    FuncScriptTextTemplate = 'FuncScriptTextTemplate',
    FsStudioParentNode = 'FsStudioParentNode',
}

interface EvalNodeContextValue {
    fetchChildren: (sessionId: string, path: string | null) => Promise<NodeItem[]>;
    renameNode: (sessionId: string, nodePath: string, newName: string) => Promise<void>;
    deleteNode: (sessionId: string, nodePath: string) => Promise<void>;
    createNode: (
        sessionId: string,
        parentPath: string | null,
        name: string,
        type: ExpressionType
    ) => Promise<void>;
    moveNode: (sessionId: string, draggedPath: string, newParentPath: string | null) => Promise<void>;
}

const EvalNodContext = createContext<EvalNodeContextValue | null>(null);

export const EvalNodeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const fetchChildren = (sessionId: string, path: string | null) => {
        return axios
            .get<NodeItem[]>(`${SERVER_URL}/api/sessions/${sessionId}/node/children`, {
                params: { nodePath: path },
            })
            .then((response) => {
                return response.data.map((item) => ({
                    ...item,
                    path: path ? `${path}.${item.name}` : item.name,
                }));
            });
    };

    const renameNode = async (sessionId: string, nodePath: string, newName: string) => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/rename`, {
            nodePath,
            newName,
        });
    };

    const deleteNode = async (sessionId: string, nodePath: string): Promise<void> => {
        await axios.delete(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
            params: { nodePath },
        });
    };
    
    const createNode = async (
        sessionId: string,
        parentPath: string | null,
        name: string,
        type: ExpressionType
    ): Promise<void> => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
            ParentNodePath: parentPath,
            Name: name,
            ExpressionType: type,
            Expression: '',
        });
    };
    
    const moveNode = async (
        sessionId: string,
        draggedPath: string,
        newParentPath: string | null
    ): Promise<void> => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/move`, {
            nodePath: draggedPath,
            newParentPath,
        });
    };

    const value: EvalNodeContextValue = {
        fetchChildren,
        renameNode,
        deleteNode,
        createNode,
        moveNode,
    };

    return <EvalNodContext.Provider value={value}>{children}</EvalNodContext.Provider>;
};

export const useEvalNod = () => {
    const context = useContext(EvalNodContext);
    if (!context) throw new Error('useEvalNod must be used within an EvalNodProvider');
    return context;
};
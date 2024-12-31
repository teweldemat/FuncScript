// FsStudioProvider.tsx
import React, {
    createContext,
    useContext,
    useEffect,
    useState,
    useCallback,
    useMemo,
  } from 'react';
  import axios from 'axios';
import { SERVER_URL } from './backend';
  
  
  // Same enum and interface definitions, just carried over:
  export enum ExpressionType {
    ClearText = 'ClearText',
    FuncScript = 'FuncScript',
    FuncScriptTextTemplate = 'FuncScriptTextTemplate',
    FsStudioParentNode = 'FsStudioParentNode',
  }
  
  export interface NodeItem {
    name: string;
    expressionType: ExpressionType;
    childrenCount: number;
    path: string | null;
    expression: string | null;
  }
  
  // We keep whatever structures we need for node-based state:
  interface NodeState {
    open: boolean;
    menuAnchorEl: HTMLElement | null;
    renameMode: boolean;
    renamedName: string;
    newInputMode: boolean;
    newName: string;
    newNodeType: ExpressionType;
    deleteItem: boolean;
    dragOver: boolean;
    children: NodeItem[];
  }
  
  // For each session we store the node tree's expanded nodes, selected node, node states, etc.
  interface SessionData {
    nodeStates: Record<string, NodeState>;
    selectedNode: string | null;
  }
  
  // We also store file-tree expansions, selected file, plus the session map:
  interface FsStudioContextValue {
    fileTreeExpandedItems: string[];
    setFileTreeExpandedItems: React.Dispatch<React.SetStateAction<string[]>>;
    selectedFileItem: string | null;
    setSelectedFileItem: React.Dispatch<React.SetStateAction<string | null>>;
  
    sessions: Record<string, SessionData>;
    setSessions: React.Dispatch<React.SetStateAction<Record<string, SessionData>>>;
  
    fetchChildren: (sessionId: string, path: string | null) => Promise<NodeItem[]>;
    renameNode: (sessionId: string, nodePath: string, newName: string) => Promise<void>;
    deleteNode: (sessionId: string, nodePath: string) => Promise<void>;
    createNode: (
      sessionId: string,
      parentPath: string | null,
      name: string,
      type: ExpressionType
    ) => Promise<void>;
    moveNode: (
      sessionId: string,
      draggedPath: string,
      newParentPath: string | null
    ) => Promise<void>;
  
    // You can place your FS calls here too if you want them centralized:
    listSubFoldersAndFiles: (
      fullPath: string
    ) => Promise<{ directories: string[]; files: string[] }>;
  
    // Utility for updating node states in a given session
    updateNodeStates: (
      sessionId: string,
      nodePath: string,
      partialState: Partial<NodeState>
    ) => void;
  }
  
  const FsStudioContext = createContext<FsStudioContextValue>({} as FsStudioContextValue);
  
  export const FsStudioProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [fileTreeExpandedItems, setFileTreeExpandedItems] = useState<string[]>([]);
    const [selectedFileItem, setSelectedFileItem] = useState<string | null>(null);
  
    // Key: sessionId => { nodeStates, selectedNode }
    const [sessions, setSessions] = useState<Record<string, SessionData>>({});
  
    // Node calls that used to live in EvalNodeProvider:
    const fetchChildren = useCallback(async (sessionId: string, path: string | null) => {
      const response = await axios.get<NodeItem[]>(
        `${SERVER_URL}/api/sessions/${sessionId}/node/children`,
        { params: { nodePath: path } }
      );
      return response.data.map((item) => ({
        ...item,
        path: path ? `${path}.${item.name}` : item.name,
      }));
    }, []);
  
    const renameNode = useCallback(
      async (sessionId: string, nodePath: string, newName: string) => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/rename`, {
          nodePath,
          newName,
        });
      },
      []
    );
  
    const deleteNode = useCallback(
      async (sessionId: string, nodePath: string) => {
        await axios.delete(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
          params: { nodePath },
        });
      },
      []
    );
  
    const createNode = useCallback(
      async (
        sessionId: string,
        parentPath: string | null,
        name: string,
        type: ExpressionType
      ) => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
          ParentNodePath: parentPath,
          Name: name,
          ExpressionType: type,
          Expression: '',
        });
      },
      []
    );
  
    const moveNode = useCallback(
      async (sessionId: string, draggedPath: string, newParentPath: string | null) => {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/move`, {
          nodePath: draggedPath,
          newParentPath,
        });
      },
      []
    );
  
    // Example FS call that used to live in NavItemComponent:
    const listSubFoldersAndFiles = useCallback(
      async (fullPath: string) => {
        const resp = await axios.get(`${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles`, {
          params: { path: fullPath },
        });
        return resp.data as { directories: string[]; files: string[] };
      },
      []
    );
  
    // Helper for updating node states
    const updateNodeStates = useCallback(
      (sessionId: string, nodePath: string, partialState: Partial<NodeState>) => {
        setSessions((prev) => {
          const session = prev[sessionId];
          if (!session) return prev;
  
          const existing = session.nodeStates[nodePath] || {
            open: false,
            menuAnchorEl: null,
            renameMode: false,
            renamedName: '',
            newInputMode: false,
            newName: '',
            newNodeType: ExpressionType.FuncScript,
            deleteItem: false,
            dragOver: false,
            children: [],
          };
          const updated = { ...existing, ...partialState };
          return {
            ...prev,
            [sessionId]: {
              ...session,
              nodeStates: {
                ...session.nodeStates,
                [nodePath]: updated,
              },
            },
          };
        });
      },
      []
    );
  
    const value = useMemo(
      () => ({
        fileTreeExpandedItems,
        setFileTreeExpandedItems,
        selectedFileItem,
        setSelectedFileItem,
  
        sessions,
        setSessions,
  
        fetchChildren,
        renameNode,
        deleteNode,
        createNode,
        moveNode,
  
        listSubFoldersAndFiles,
  
        updateNodeStates,
      }),
      [
        fileTreeExpandedItems,
        selectedFileItem,
        sessions,
        fetchChildren,
        renameNode,
        deleteNode,
        createNode,
        moveNode,
        listSubFoldersAndFiles,
        updateNodeStates,
      ]
    );
  
    return <FsStudioContext.Provider value={value}>{children}</FsStudioContext.Provider>;
  };
  
  export const useFsStudio = () => {
    const context = useContext(FsStudioContext);
    if (!context) throw new Error('useFsStudio must be used within a FsStudioProvider');
    return context;
  };
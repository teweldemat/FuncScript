// FsStudioProvider.tsx
import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { Box } from '@mui/material';
import axios from 'axios';
import Navigation from './components/Navigation';
import ExecutionView from './components/ExecutionView';
import { SERVER_URL, SERVER_WS_URL } from './backend';

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

/**
 * Extend context to hold nodeEvaluations:
 *   nodeEvaluations[nodePath] = string (or any type you want)
 */
interface FsStudioContextValue {
  currentSessionId: string | null;
  currentFilePath: string | null;
  setCurrentFilePath: React.Dispatch<React.SetStateAction<string | null>>;
  currentNode: string | null;
  handleNodeSelect: (nodePath: string | null) => void;
  handleSelectedFile: (filePath: string) => void;

  nodeEvaluations: Record<string, string>;
  updateNodeEvaluation: (nodePath: string, result: string) => void;
}

const FsStudioContext = createContext<FsStudioContextValue>({} as FsStudioContextValue);

export const FsStudioProvider: React.FC = () => {
  const [sessionMap, setSessionMap] = useState<{ [path: string]: string }>({});
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  const [currentFilePath, setCurrentFilePath] = useState<string | null>(null);
  const [currentNode, setCurrentNode] = useState<string | null>(null);

  // This is our new "evaluation results" store:
  const [nodeEvaluations, setNodeEvaluations] = useState<Record<string, string>>({});

  // Called whenever the user selects a file from Navigation
  const handleSelectedFile = (filePath: string) => {
    setCurrentFilePath(filePath);
    if (sessionMap[filePath]) {
      setCurrentSessionId(sessionMap[filePath]);
    } else {
      createSession(filePath);
    }
  };

  // Called whenever the user selects a node inside ExecutionView
  const handleNodeSelect = (nodePath: string | null) => {
    setCurrentNode(nodePath);
  };

  const createSession = (filePath: string) => {
    axios
      .post(`${SERVER_URL}/api/sessions/create`, { fromFile: filePath })
      .then((response) => {
        const newSessionId = response.data.sessionId;
        setSessionMap((prevMap) => ({
          ...prevMap,
          [filePath]: newSessionId,
        }));
        setCurrentSessionId(newSessionId);
      })
      .catch((error) => {
        console.error('Failed to create session:', error);
        alert('Failed to create session: ' + error.message);
      });
  };

  /**
   * Function used to update the node's evaluation result in our context-level store.
   */
  const updateNodeEvaluation = (nodePath: string, result: string) => {
    setNodeEvaluations((prev) => ({
      ...prev,
      [nodePath]: result,
    }));
  };

  /**
   * Open WebSocket at top-level, watch for evaluation results from the backend,
   * and store them via `updateNodeEvaluation`.
   */
  useEffect(() => {
    if (!currentSessionId) return;
    const websocket = new WebSocket(SERVER_WS_URL);

    websocket.onopen = () => {
      console.log('WebSocket connected');
      // Possibly inform the server which session we want to join, if needed
    };

    websocket.onmessage = (event) => {
      const msg = JSON.parse(event.data);
      switch (msg.cmd) {
        // You could also handle log/markdown messages here if you want them top-level
        case 'evaluation_success': {
          // Example data from server: { sessionId, nodePath, result }
          const { nodePath, result } = msg.data;
          console.log('root got result')
          // Update context with new evaluation result
          if (nodePath) {
            updateNodeEvaluation(nodePath, result);
          }
          break;
        }
        case 'evaluation_error':
          // handle error
          break;
        // handle other messages if needed
        default:
          break;
      }
    };

    websocket.onerror = (err) => {
      console.error('WebSocket error:', err);
    };

    // Cleanup
    return () => {
      websocket.close();
    };
  }, [currentSessionId]);

  const contextValue: FsStudioContextValue = {
    currentSessionId,
    currentFilePath,
    setCurrentFilePath,
    currentNode,
    handleNodeSelect,
    handleSelectedFile,
    nodeEvaluations,
    updateNodeEvaluation,
  };

  return (
    <FsStudioContext.Provider value={contextValue}>
      <Box sx={{ display: 'flex', width: '100%', height: '100%' }}>
        <Navigation onSelected={handleSelectedFile} initiallySelectedPath={null} />
        {currentSessionId && (
          <ExecutionView
            sessionId={currentSessionId}
            initiallySelectedNode={currentNode}
            onNodeSelect={handleNodeSelect}
          />
        )}
      </Box>
    </FsStudioContext.Provider>
  );
};

export const useFsStudio = () => {
  const context = useContext(FsStudioContext);
  if (!context) {
    throw new Error('useFsStudio must be used within a FsStudioProvider');
  }
  return context;
};
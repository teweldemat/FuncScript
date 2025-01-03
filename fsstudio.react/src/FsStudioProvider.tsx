import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { Box } from '@mui/material';
import axios from 'axios';
import Navigation from './components/Navigation';
import ExecutionView from './components/ExecutionView';
import { SERVER_URL } from './backend';

/**
 * You can keep or rename these, or move them to their own file. 
 * Leaving them here to keep the example self-contained.
 */
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

interface FsStudioContextValue {
  currentSessionId: string | null;
  currentFilePath: string | null;
  setCurrentFilePath: React.Dispatch<React.SetStateAction<string | null>>;
  currentNode: string | null;
  handleNodeSelect: (nodePath: string | null) => void;
  handleSelectedFile: (filePath: string) => void;
}

const FsStudioContext = createContext<FsStudioContextValue>({} as FsStudioContextValue);

/** 
 * This FsStudioProvider now also renders Navigation and ExecutionView as siblings.
 * We move the "session" logic from App into here, so children can use it from context.
 */
export const FsStudioProvider: React.FC = () => {
  const [sessionMap, setSessionMap] = useState<{ [path: string]: string }>({});
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  const [currentFilePath, setCurrentFilePath] = useState<string | null>(null);
  const [currentNode, setCurrentNode] = useState<string | null>(null);

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

  const contextValue: FsStudioContextValue = {
    currentSessionId,
    currentFilePath,
    setCurrentFilePath,
    currentNode,
    handleNodeSelect,
    handleSelectedFile,
  };

  return (
    <FsStudioContext.Provider value={contextValue}>
      <Box sx={{ display: 'flex', width: '100%', height: '100%' }}>
        <Navigation
          onSelected={handleSelectedFile}
          initiallySelectedPath={null}
        />
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
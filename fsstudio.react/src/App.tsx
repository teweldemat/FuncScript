// App.tsx
import React, { useEffect, useState } from 'react';
import { CssBaseline, Box } from '@mui/material';
import Navigation from './navigator/Navigation';
import ExecutionView from './execution/ExecutionView';
import axios from 'axios';
import { SERVER_URL } from './backend';

const App: React.FC = () => {
  const [sessionMap, setSessionMap] = useState<{ [path: string]: string }>({});
  const [currentSessionId, setCurrentSessionId] = useState<string | null>(null);
  const [currentFilePath, setCurrentFilePath] = useState<string | null>(null);
  const [currentNode, setCurrentNode] = useState<string | null>(null);
  const [expandedNodes, setExpandedNodes] = useState<string[]>([]);

  const handleSelected = (filePath: string) => {
    setCurrentFilePath(filePath);
    if (sessionMap[filePath]) {
      setCurrentSessionId(sessionMap[filePath]);
    } else {
      createSession(filePath);
    }
    saveUiState({ ExpandedNodes: expandedNodes });
  };

  const handleNodeSelect = (node: string | null) => {
    setCurrentNode(node);
    saveUiState({ ExpandedNodes: expandedNodes });
  };

  const createSession = (filePath: string) => {
    axios
      .post(`${SERVER_URL}/api/sessions/create`, { fromFile: filePath })
      .then(response => {
        const newSessionId = response.data.sessionId;
        setSessionMap(prevMap => ({
          ...prevMap,
          [filePath]: newSessionId
        }));
        setCurrentSessionId(newSessionId);
      })
      .catch(error => {
        console.error('Failed to create session:', error);
        alert('Failed to create session: ' + error.message);
      });
  };

  // DISABLED: loadUiState no longer loads the last selected file/variable
  const loadUiState = () => {
    const uiStateJson = localStorage.getItem('uiState');
    if (uiStateJson) {
      const uiState = JSON.parse(uiStateJson);
      setExpandedNodes(uiState.ExpandedNodes || []);
    }
  };

  // DISABLED: saveUiState no longer stores selected file/variable
  const saveUiState = (uiState: { ExpandedNodes: string[] }) => {
    const newUiState = {
      ExpandedNodes: uiState.ExpandedNodes
    };
    localStorage.setItem('uiState', JSON.stringify(newUiState));
  };

  useEffect(() => {
    loadUiState();
  }, []);

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <Navigation onSelected={handleSelected} initiallySelectedPath={null} />
      {currentSessionId && (
        <ExecutionView
          sessionId={currentSessionId}
          initiallySelectedNode={currentNode}
          onNodeSelect={handleNodeSelect}
        />
      )}
    </Box>
  );
};

export default App;
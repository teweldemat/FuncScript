import React, { useEffect, useState, useCallback } from 'react';
import { Box, CircularProgress } from '@mui/material';
import { useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeItem from './ExpressionNodeItem';

interface ExpressionNodeTreeProps {
  sessionId: string;
  rootNodePath: string | null; 
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
}

const ExpressionNodeTree: React.FC<ExpressionNodeTreeProps> = ({
  sessionId,
  rootNodePath,
  onSelect,
  selectedNode,
}) => {
  const { loadChildNodeList, sessions } = useExecutionSession() || {};
  const [childNodes, setChildNodes] = useState<string[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const loadChildren = useCallback(async () => {
    if (!loadChildNodeList || !sessionId) return;
    setLoading(true);
    try {
      const nodes = await loadChildNodeList(sessionId, rootNodePath??'');
      // Return just the names (or entire objects if you want)
      setChildNodes(nodes.map((n) => n.name));
    } catch (error) {
      console.error('Failed to load child nodes:', error);
    } finally {
      setLoading(false);
    }
  }, [sessionId, rootNodePath, loadChildNodeList]);

  useEffect(() => {
    loadChildren();
  }, [loadChildren]);

  if (loading) {
    return (
      <Box sx={{ p: 1 }}>
        <CircularProgress size="1rem" />
        &nbsp;Loading...
      </Box>
    );
  }

  if (!childNodes.length) {
    return <Box sx={{ p: 1 }}>No child nodes found.</Box>;
  }

  return (
    <Box>
      {childNodes.map((childName) => {
        const nodePath = rootNodePath
          ? `${rootNodePath}.${childName}`
          : childName;
        return (
          <ExpressionNodeItem
            key={nodePath}
            sessionId={sessionId}
            nodePath={nodePath}
            onSelect={onSelect}
            selectedNode={selectedNode}
          />
        );
      })}
    </Box>
  );
};

export default ExpressionNodeTree;

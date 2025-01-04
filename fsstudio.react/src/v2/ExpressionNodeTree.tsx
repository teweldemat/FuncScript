import React, { useEffect, useState, useCallback } from 'react';
import { Box, CircularProgress } from '@mui/material';
import { NodeState, useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeItem from './ExpressionNodeItem';

interface ExpressionNodeTreeProps {
  sessionId: string;
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
}

const ExpressionNodeTree: React.FC<ExpressionNodeTreeProps> = ({
  sessionId,
  onSelect,
  selectedNode,
}) => {
  const { sessions } = useExecutionSession() || {};
  
  const session=sessions==null?null:sessions[sessionId];

  
  if (session?.nodes==null) {
    return (
      <Box sx={{ p: 1 }}>
        <CircularProgress size="1rem" />
        &nbsp;Loading...
      </Box>
    );
  }

 const childNodes=session.nodes;
  if (childNodes==null) {
    return <Box sx={{ p: 1 }}>No child nodes found.</Box>;
  }

  return (
    <Box>
      {Object.values(childNodes).map((child) => (
        <ExpressionNodeItem
          key={child.name}
          sessionId={sessionId}
          nodePath={child.name}
          onSelect={onSelect}
          selectedNode={selectedNode}
          nodeInfo={child}
        />
      ))}
    </Box>
  );
};

export default ExpressionNodeTree;

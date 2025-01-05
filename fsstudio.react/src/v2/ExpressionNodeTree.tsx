import React, { useEffect, useState, useCallback } from 'react';
import { Box, CircularProgress } from '@mui/material';
import { NodeState, SessionState, useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeItem from './ExpressionNodeItem';

interface ExpressionNodeTreeProps {
  session: SessionState;
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
}

const ExpressionNodeTree: React.FC<ExpressionNodeTreeProps> = ({
  session,
  onSelect,
  selectedNode,
}) => {
  

  
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
          session={session}
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

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
  

  
  if (session?.rootNode.childNodes==null) {
    return (
      <Box sx={{ p: 1 }}>
        <CircularProgress size="1rem" />
        &nbsp;Loading...
      </Box>
    );
  }

 const childNodes=session?.rootNode?.childNodes;
  if (childNodes==null) {
    return <Box sx={{ p: 1 }}>No child nodes found.</Box>;
  }

  return (
        <ExpressionNodeItem
          session={session}
          nodePath={''}
          onSelect={onSelect}
          selectedNode={selectedNode}
          nodeInfo={session.rootNode}
        />
      );
};

export default ExpressionNodeTree;

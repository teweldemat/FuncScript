import React from 'react';
import {
  Box,
  Collapse,
  IconButton,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  CircularProgress
} from '@mui/material';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import DescriptionIcon from '@mui/icons-material/Description';
import FunctionsIcon from '@mui/icons-material/Functions';
import CodeIcon from '@mui/icons-material/Code';
import FolderIcon from '@mui/icons-material/Folder';
import { NodeState, SessionState, useExecutionSession } from './ExecutionSessionProvider';
import { ExpressionType } from '../FsStudioProvider';

interface ExpressionNodeItemProps {
  session: SessionState;
  nodePath?: string;
  nodeInfo: NodeState;
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
}

function getIconForExpressionType(expressionType: ExpressionType) {
  switch (expressionType) {
    case ExpressionType.ClearText:
      return <DescriptionIcon />;
    case ExpressionType.FuncScript:
      return <FunctionsIcon />;
    case ExpressionType.FuncScriptTextTemplate:
      return (
        <Box sx={{ display: 'inline-flex', alignItems: 'center' }}>
          <FunctionsIcon sx={{ marginRight: '4px' }} />
          <DescriptionIcon />
        </Box>
      );
    case ExpressionType.FsStudioParentNode:
      return <CodeIcon />;
    default:
      return <FolderIcon />;
  }
}

const ExpressionNodeItem: React.FC<ExpressionNodeItemProps> = ({
  session,
  nodePath,
  nodeInfo,
  onSelect,
  selectedNode
}) => {
  const { loadChildNodeList, toggleNodeExpanded } = useExecutionSession() || {};
  console.log('path: '+nodePath)
  console.log(session?.expandedNodes)
  const isOpen =!nodePath || !!session?.expandedNodes[nodePath];
  console.log('Is open: '+isOpen)
  const isEvaluating = nodeInfo.evaluating;
  const displayName = nodeInfo.name + (isEvaluating ? ' (evaluating ..)' : '');
  const expressionTypeIcon = getIconForExpressionType(nodeInfo.expressionType);

  const handleToggle = async (e: React.MouseEvent<HTMLButtonElement>) => {
    if(!nodePath)
        return;
    e.stopPropagation();
    toggleNodeExpanded?.(session, nodePath);
    if (!isOpen ) {
        console.log('is open now')
      await loadChildNodeList?.(session, nodePath);
    }
  };
  return (
    <>
      <ListItem
        dense
        onClick={(e) => {
          e.stopPropagation();
          if(!nodePath)
            return;
          onSelect(nodePath);
        }}
        sx={{
          cursor: 'pointer',
          backgroundColor: selectedNode === nodePath ? 'lightgray' : 'inherit',
        }}
      >
        {nodeInfo.childrenCount > 0 && (
          <IconButton
            onClick={handleToggle}
            size="small"
            sx={{ mr: 1 }}
          >
            {isOpen ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        )}
        <ListItemIcon>
          {isEvaluating ? <CircularProgress size="1rem" /> : expressionTypeIcon}
        </ListItemIcon>
        <ListItemText primary={displayName} />
      </ListItem>

      {nodeInfo.childrenCount > 0 && nodeInfo.childNodes && (
        <Collapse in={isOpen} timeout="auto" unmountOnExit>
          <List component="div" disablePadding dense>
            {nodeInfo.childNodes.map((childNodeInfo) => {
              const childPath = `${nodePath}.${childNodeInfo.name}`;
              return (
                <Box key={childPath} pl={4}>
                  <ExpressionNodeItem
                    session={session}
                    nodePath={childPath}
                    nodeInfo={childNodeInfo}
                    onSelect={onSelect}
                    selectedNode={selectedNode}
                  />
                </Box>
              );
            })}
          </List>
        </Collapse>
      )}
    </>
  );
};

export default ExpressionNodeItem;

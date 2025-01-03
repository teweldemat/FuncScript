import React, { useEffect, useState, useCallback } from 'react';
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
import { useExecutionSession } from './ExecutionSessionProvider';
import { ExpressionType } from '../FsStudioProvider';

interface ExpressionNodeItemProps {
  sessionId: string;
  nodePath: string;
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
  sessionId,
  nodePath,
  onSelect,
  selectedNode,
}) => {
  const {
    sessions,
    loadNode,
    loadChildNodeList,
    toggleNodeExpanded
  } = useExecutionSession() || {};

  const [childNames, setChildNames] = useState<string[]>([]);
  const [loadingChildren, setLoadingChildren] = useState(false);

  const session = sessions?.[sessionId];
  const nodeState = session?.nodes[nodePath];

  const isOpen = !!session?.expandedNodes[nodePath];
  const isEvaluating = !!nodeState?.evaluating;
  const displayName = nodeState
    ? nodeState.name + (isEvaluating ? ' (evaluating ..)' : '')
    : nodePath;

  const fetchChildren = useCallback(async () => {
    if (!loadChildNodeList || !sessionId || !nodePath || !nodeState) return;
    setLoadingChildren(true);
    try {
      const children = await loadChildNodeList(sessionId, nodePath);
      setChildNames(children.map((c) => c.name));
    } catch (err) {
      console.error('Error loading children', err);
    } finally {
      setLoadingChildren(false);
    }
  }, [sessionId, nodePath, nodeState, loadChildNodeList]);

  useEffect(() => {
    if (!nodeState) {
      (async () => {
        try {
          await loadNode?.(sessionId, nodePath);
        } catch (e) {
          console.error('Failed to load node:', e);
        }
      })();
    }
  }, [nodeState, sessionId, nodePath, loadNode]);

  useEffect(() => {
    if (isOpen && !childNames.length) {
      fetchChildren();
    }
  }, [isOpen, childNames.length, fetchChildren]);

  if (!nodeState) {
    return (
      <ListItem dense>
        <CircularProgress size="1rem" />
        &nbsp;Loading node {nodePath}...
      </ListItem>
    );
  }

  const expressionTypeIcon = getIconForExpressionType(nodeState.expressionType);

  return (
    <>
      <ListItem
        dense
        onClick={(e) => {
          e.stopPropagation();
          onSelect(nodePath);
        }}
        sx={{
          cursor: 'pointer',
          backgroundColor: selectedNode === nodePath ? 'lightgray' : 'inherit',
        }}
      >
        {nodeState.childrenCount > 0 && (
          <IconButton onClick={(e) => {
            e.stopPropagation();
            toggleNodeExpanded?.(sessionId, nodePath);
          }} size="small" sx={{ mr: 1 }}>
            {isOpen ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        )}
        <ListItemIcon>
          {isEvaluating ? <CircularProgress size="1rem" /> : expressionTypeIcon}
        </ListItemIcon>
        <ListItemText primary={displayName} />
      </ListItem>

      <Collapse in={isOpen} timeout="auto" unmountOnExit>
        <List component="div" disablePadding dense>
          {loadingChildren && (
            <ListItem>
              <CircularProgress size="1rem" />
              &nbsp;Loading children...
            </ListItem>
          )}
          {childNames.map((childName) => {
            const childPath = `${nodePath}.${childName}`;
            return (
              <Box key={childPath} pl={4}>
                <ExpressionNodeItem
                  sessionId={sessionId}
                  nodePath={childPath}
                  onSelect={onSelect}
                  selectedNode={selectedNode}
                />
              </Box>
            );
          })}
        </List>
      </Collapse>
    </>
  );
};

export default ExpressionNodeItem;
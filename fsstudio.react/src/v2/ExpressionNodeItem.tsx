import React, { useState } from 'react';
import {
  Box,
  Collapse,
  IconButton,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  CircularProgress,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  TextField,
} from '@mui/material';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import DescriptionIcon from '@mui/icons-material/Description';
import FunctionsIcon from '@mui/icons-material/Functions';
import CodeIcon from '@mui/icons-material/Code';
import FolderIcon from '@mui/icons-material/Folder';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import { NodeState, SessionState, useExecutionSession } from './ExecutionSessionProvider';
import { ExpressionType } from '../FsStudioProvider';

interface ExpressionNodeItemProps {
  session: SessionState;
  nodePath?: string;
  nodeInfo: NodeState;
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
}

function getParentPath(fullPath: string) {
  const idx = fullPath.lastIndexOf('.');
  return idx > 0 ? fullPath.slice(0, idx) : null;
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
  selectedNode,
}) => {
  const {
    loadChildNodeList,
    toggleNodeExpanded,
    createNode,
    removeNode,
    renameNode,
  } = useExecutionSession() || {};

  const [menuAnchorEl, setMenuAnchorEl] = useState<HTMLElement | null>(null);
  const [renameMode, setRenameMode] = useState(false);
  const [renamedName, setRenamedName] = useState(nodeInfo.name);
  const [newInputMode, setNewInputMode] = useState(false);
  const [newName, setNewName] = useState('');
  const [deleteItem, setDeleteItem] = useState(false);

  const isOpen = !nodePath ? true : !!session?.expandedNodes[nodePath];
  const isEvaluating = nodeInfo.evaluating;
  const displayName = nodeInfo.name + (isEvaluating ? ' (evaluating ..)' : '');
  const expressionTypeIcon = getIconForExpressionType(nodeInfo.expressionType);

  const handleToggle = async (e: React.MouseEvent<HTMLButtonElement>) => {
    e.stopPropagation();
    if (!nodePath) return;
    toggleNodeExpanded?.(session, nodePath);
    if (!isOpen) {
      await loadChildNodeList?.(session, nodePath);
    }
  };

  const handleMenuClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    e.stopPropagation();
    setMenuAnchorEl(e.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchorEl(null);
  };

  const handleMenuAction = async (action: string) => {
    handleCloseMenu();
    switch (action) {
      case 'add-standard':
      case 'add-text':
      case 'add-text-template':
        setNewInputMode(true);
        break;
      case 'rename':
        setRenameMode(true);
        break;
      case 'delete':
        setDeleteItem(true);
        break;
      default:
        break;
    }
  };

  const handleClickItem = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (!nodePath || nodeInfo.childrenCount > 0) return;
    onSelect(nodePath);
  };

  const handleAddItem = async () => {
    if (!nodePath && !session) return;
    if (newName.trim() === '') return;
    const parentPath = nodePath || null;
    let newType = ExpressionType.FuncScript;
    if (menuAnchorEl?.textContent?.toLowerCase().includes('text template')) {
      newType = ExpressionType.FuncScriptTextTemplate;
    } else if (menuAnchorEl?.textContent?.toLowerCase().includes('text')) {
      newType = ExpressionType.ClearText;
    }
    await createNode?.(session, parentPath, newName, '', newType);
    setNewName('');
    setNewInputMode(false);
    if (nodePath) {
      await loadChildNodeList?.(session, nodePath);
    } else {
      await loadChildNodeList?.(session, null);
    }
  };

  const handleDeleteItem = async () => {
    if (!nodePath || !session) return;
    await removeNode?.(session, nodePath);
    setDeleteItem(false);
    const parent = getParentPath(nodePath);
    await loadChildNodeList?.(session, parent);
    if (selectedNode === nodePath) {
      onSelect(null);
    }
  };

  const handleApplyRename = async () => {
    if (!nodePath || !session || renamedName.trim() === '') {
      setRenameMode(false);
      return;
    }
    await renameNode?.(session, nodePath, renamedName);
    setRenameMode(false);
    const parent = getParentPath(nodePath);
    await loadChildNodeList?.(session, parent);
  };

  return (
    <>
      <ListItem
        dense
        onClick={handleClickItem}
        sx={{
          cursor: 'pointer',
          backgroundColor: selectedNode === nodePath ? 'lightgray' : 'inherit',
        }}
      >
        <IconButton onClick={handleMenuClick} size="small" sx={{ mr: 1 }}>
          <MoreVertIcon />
        </IconButton>
        <Menu
          anchorEl={menuAnchorEl}
          open={Boolean(menuAnchorEl)}
          onClose={handleCloseMenu}
        >
          {!nodePath
            ? ['Add Standard', 'Add Text', 'Add Text Template'].map((mi) => (
                <MenuItem
                  key={mi}
                  onClick={(event) => {
                    event.stopPropagation();
                    handleMenuAction(mi.toLowerCase().replaceAll(' ', '-'));
                  }}
                >
                  {mi}
                </MenuItem>
              ))
            : ['Add Standard', 'Add Text', 'Add Text Template', 'Rename', 'Delete'].map(
                (mi) => (
                  <MenuItem
                    key={mi}
                    onClick={(event) => {
                      event.stopPropagation();
                      handleMenuAction(mi.toLowerCase().replaceAll(' ', '-'));
                    }}
                  >
                    {mi}
                  </MenuItem>
                )
              )}
        </Menu>

        {nodeInfo.childrenCount > 0 && (
          <IconButton onClick={handleToggle} size="small" sx={{ mr: 1 }}>
            {isOpen ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        )}

        <ListItemIcon>
          {isEvaluating ? <CircularProgress size="1rem" /> : expressionTypeIcon}
        </ListItemIcon>

        {!renameMode ? (
          <ListItemText primary={displayName} />
        ) : (
          <TextField
            size="small"
            value={renamedName}
            onChange={(e) => setRenamedName(e.target.value)}
            autoFocus
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleApplyRename();
              if (e.key === 'Escape') setRenameMode(false);
            }}
          />
        )}
      </ListItem>

      {newInputMode && (
        <Box pl={4}>
          <TextField
            size="small"
            label="Enter name"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') handleAddItem();
              if (e.key === 'Escape') {
                setNewInputMode(false);
                setNewName('');
              }
            }}
            autoFocus
          />
        </Box>
      )}

      {nodeInfo.childrenCount > 0 && nodeInfo.childNodes && (
        <Collapse in={isOpen} timeout="auto" unmountOnExit>
          <List component="div" disablePadding dense>
            {nodeInfo.childNodes.map((childNodeInfo) => {
              const childPath = nodePath ? `${nodePath}.${childNodeInfo.name}` : childNodeInfo.name;
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

      <Dialog open={deleteItem} onClose={() => setDeleteItem(false)}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete {nodeInfo.name}? This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteItem(false)} color="primary">
            Cancel
          </Button>
          <Button
            onClick={handleDeleteItem}
            color="primary"
            autoFocus
          >
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default ExpressionNodeItem;

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
  Tooltip,
} from '@mui/material';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import DescriptionIcon from '@mui/icons-material/Description';
import FunctionsIcon from '@mui/icons-material/Functions';
import CodeIcon from '@mui/icons-material/Code';
import FolderIcon from '@mui/icons-material/Folder';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import TextFieldsIcon from '@mui/icons-material/TextFields';
import LibraryAddIcon from '@mui/icons-material/LibraryAdd';
import {
  ExpressionType,
  NodeState,
  SessionState,
  useExecutionSession,
} from './SessionContext';

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

interface ExpressionNodeItemProps {
  session: SessionState;
  nodePath?: string;
  nodeInfo: NodeState;
  onSelect: (nodePath: string | null) => void;
  selectedNode: string | null;
  readonly: boolean;
}

const ExpressionNodeItem: React.FC<ExpressionNodeItemProps> = ({
  session,
  nodePath,
  nodeInfo,
  onSelect,
  selectedNode,
  readonly,
}) => {
  const {
    loadChildNodeList,
    toggleNodeExpanded,
    createNode,
    removeNode,
    renameNode,
    moveNode,
  } = useExecutionSession() || {};

  // -------------------------------------------------------------------------
  // 1) Track which menu action was clicked
  // -------------------------------------------------------------------------
  const [menuAction, setMenuAction] = useState<string | null>(null);
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
    if (readonly) return;
    e.stopPropagation();
    setMenuAnchorEl(e.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchorEl(null);
  };

  // -------------------------------------------------------------------------
  // 2) Set the menuAction directly instead of trying to read from the DOM
  // -------------------------------------------------------------------------
  const handleMenuAction = async (action: string) => {
    if (readonly) return;
    setMenuAction(action);
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

  // -------------------------------------------------------------------------
  // 3) Use menuAction to decide the ExpressionType
  // -------------------------------------------------------------------------
  const handleAddItem = async () => {
    if (!session || readonly || newName.trim() === '' || !menuAction) return;

    const parentPath = nodePath || null;
    let newType: ExpressionType;

    switch (menuAction) {
      case 'add-text':
        newType = ExpressionType.ClearText;
        break;
      case 'add-text-template':
        newType = ExpressionType.FuncScriptTextTemplate;
        break;
      case 'add-standard':
      default:
        newType = ExpressionType.FuncScript;
        break;
    }

    // Create the new node
    await createNode?.(session, parentPath, newName, '', newType);

    // Reset states
    setNewName('');
    setNewInputMode(false);
    setMenuAction(null);

    // Reload children
    if (nodePath) {
      await loadChildNodeList?.(session, nodePath);
    } else {
      await loadChildNodeList?.(session, null);
    }
  };

  const handleDeleteItem = async () => {
    if (!nodePath || !session || readonly) return;
    await removeNode?.(session, nodePath);
    setDeleteItem(false);
    // Reload the parent after deletion
    const parentPath = nodePath.includes('.') ? nodePath.split('.').slice(0, -1).join('.') : null;
    await loadChildNodeList?.(session, parentPath);
    // If we were viewing the deleted node, deselect it
    if (selectedNode === nodePath) {
      onSelect(null);
    }
  };

  const handleApplyRename = async () => {
    if (!nodePath || !session || renamedName.trim() === '' || readonly) {
      setRenameMode(false);
      return;
    }
    await renameNode?.(session, nodePath, renamedName);
    setRenameMode(false);
    // Reload the parent so we see the updated name
    const parentPath = nodePath.includes('.') ? nodePath.split('.').slice(0, -1).join('.') : null;
    await loadChildNodeList?.(session, parentPath);
  };

  const handleDragStart = (e: React.DragEvent) => {
    if (!nodePath || readonly) return;
    e.dataTransfer.setData('text/plain', nodePath);
  };

  const handleDragOver = (e: React.DragEvent) => {
    if (readonly) return;
    e.preventDefault();
  };

  const handleDrop = async (e: React.DragEvent) => {
    if (readonly) return;
    e.preventDefault();
    const sourcePath = e.dataTransfer.getData('text/plain');
    if (!sourcePath || !session || sourcePath === nodePath) return;

    // If this node is a folder-like node, we can drop inside it;
    // otherwise, we drop it alongside (its parent).
    const canContainChildren = nodeInfo.childrenCount >= 0;
    const newParentPath = canContainChildren
      ? nodePath
      : nodePath?.split('.').slice(0, -1).join('.') || null;

    if (!newParentPath) return;
    await moveNode?.(session, sourcePath, newParentPath);
  };

  return (
    <>
      <ListItem
        dense
        onClick={handleClickItem}
        draggable={!!nodePath && !readonly}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDrop={handleDrop}
        sx={{
          cursor: 'pointer',
          backgroundColor: selectedNode === nodePath ? 'lightgray' : 'inherit',
        }}
      >
        {/* Top-level add buttons, displayed if there's NO nodePath */}
        {!nodePath && !readonly && (
          <Box sx={{ display: 'flex', alignItems: 'center', mr: 1 }}>
            <Tooltip title="Add Standard">
              <IconButton
                onClick={(e) => {
                  e.stopPropagation();
                  handleMenuAction('add-standard');
                }}
                size="small"
                sx={{ mr: 1 }}
              >
                <AddCircleOutlineIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Add Text">
              <IconButton
                onClick={(e) => {
                  e.stopPropagation();
                  handleMenuAction('add-text');
                }}
                size="small"
                sx={{ mr: 1 }}
              >
                <TextFieldsIcon />
              </IconButton>
            </Tooltip>
            <Tooltip title="Add Text Template">
              <IconButton
                onClick={(e) => {
                  e.stopPropagation();
                  handleMenuAction('add-text-template');
                }}
                size="small"
              >
                <LibraryAddIcon />
              </IconButton>
            </Tooltip>
          </Box>
        )}

        {nodePath && (
          <IconButton
            onClick={handleMenuClick}
            size="small"
            sx={{ mr: 1 }}
            disabled={readonly}
          >
            <MoreVertIcon />
          </IconButton>
        )}

        <Menu
          anchorEl={menuAnchorEl}
          open={Boolean(menuAnchorEl) && !readonly}
          onClose={handleCloseMenu}
        >
          {!nodePath
            ? ['Add Standard', 'Add Text', 'Add Text Template'].map((mi) => (
                <MenuItem
                  key={mi}
                  onClick={(event) => {
                    event.stopPropagation();
                    // Translate the text into the action string you want.
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

        {nodePath && nodeInfo.childrenCount > 0 && (
          <IconButton onClick={handleToggle} size="small" sx={{ mr: 1 }}>
            {isOpen ? <ExpandLess /> : <ExpandMore />}
          </IconButton>
        )}

        {nodePath && (
          <ListItemIcon>
            {isEvaluating ? <CircularProgress size="1rem" /> : expressionTypeIcon}
          </ListItemIcon>
        )}

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
        <Box pl={nodePath ? 4 : 0}>
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
                setMenuAction(null);
              }
            }}
            autoFocus
          />
        </Box>
      )}

      {nodeInfo.childrenCount > 0 && nodeInfo.childNodes && (
        <Collapse in={nodePath ? isOpen : true} timeout="auto" unmountOnExit>
          <List component="div" disablePadding dense sx={{ pl: nodePath ? 4 : 0 }}>
            {nodeInfo.childNodes.map((childNodeInfo) => {
              const childPath = nodePath
                ? `${nodePath}.${childNodeInfo.name}`
                : childNodeInfo.name;
              return (
                <Box key={childPath} pl={nodePath ? 4 : 0}>
                  <ExpressionNodeItem
                    session={session}
                    nodePath={childPath}
                    nodeInfo={childNodeInfo}
                    onSelect={onSelect}
                    selectedNode={selectedNode}
                    readonly={readonly}
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
          <Button onClick={handleDeleteItem} color="primary" autoFocus>
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default ExpressionNodeItem;

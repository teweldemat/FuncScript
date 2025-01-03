import React from 'react';
import {
  ListItem,
  ListItemIcon,
  ListItemText,
  IconButton,
  Box,
  TextField,
  Menu,
  MenuItem,
  Collapse,
  List,
  CircularProgress,
} from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import FolderIcon from '@mui/icons-material/Folder';
import DescriptionIcon from '@mui/icons-material/Description';
import FunctionsIcon from '@mui/icons-material/Functions';
import CodeIcon from '@mui/icons-material/Code';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { NodeItem, ExpressionType } from '../FsStudioProvider';

interface EvalNodeComponentProps {
  node: NodeItem;
  nodeStates: Record<string, any>;
  onStateChange: (nodePath: string, updatedState: Partial<any>) => void;
  onSelect: (node: NodeItem) => void;
  onToggleExpand: (nodePath: string) => void;
  onApplyRename: (node: NodeItem) => void;
  onDeleteItem: (node: NodeItem) => void;
  onAddItem: (node: NodeItem) => void;
  onDragStart: (draggedNodePath: string, e: React.DragEvent) => void;
  onDragOver: (nodePath: string, e: React.DragEvent) => void;
  onDragLeave: (nodePath: string) => void;
  onDrop: (node: NodeItem, e: React.DragEvent) => void;
  selectedNode: string | null;
}

const EvalNodeComponent: React.FC<EvalNodeComponentProps> = ({
  node,
  nodeStates,
  onStateChange,
  onSelect,
  onToggleExpand,
  onApplyRename,
  onDeleteItem,
  onAddItem,
  onDragStart,
  onDragOver,
  onDragLeave,
  onDrop,
  selectedNode,
}) => {
  const nodePathKey = node.path || 'root';
  const nodeState = nodeStates[nodePathKey] || {};
  const {
    open,
    renameMode,
    renamedName,
    newInputMode,
    newName,
    dragOver: isDragOver,
    children,
    evaluating,
  } = nodeState;

  const [menuAnchorEl, setMenuAnchorEl] = React.useState<HTMLElement | null>(null);

  const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    setMenuAnchorEl(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setMenuAnchorEl(null);
  };

  const handleMenuAction = (action: string) => {
    handleCloseMenu();
    switch (action) {
      case 'add-standard':
      case 'add-text':
      case 'add-text-template':
        onStateChange(nodePathKey, { newInputMode: true });
        break;
      case 'rename':
        onStateChange(nodePathKey, { renameMode: true });
        break;
      case 'delete':
        onStateChange(nodePathKey, { deleteItem: true });
        break;
      default:
        break;
    }
  };

  function getIconForExpressionType() {
    switch (node.expressionType) {
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

  const isRoot = !node.path;
  const menuItems = isRoot
    ? ['Add Standard', 'Add Text', 'Add Text Template']
    : ['Add Standard', 'Add Text', 'Add Text Template', 'Rename', 'Delete'];

  return (
    <>
      <ListItem
        dense
        onClick={(e) => {
          e.stopPropagation();
          onSelect(node);
        }}
        draggable={!isRoot}
        onDragStart={(e) => onDragStart(nodePathKey, e)}
        onDragOver={(e) => onDragOver(nodePathKey, e)}
        onDragLeave={() => onDragLeave(nodePathKey)}
        onDrop={(e) => onDrop(node, e)}
        sx={{
          display: 'flex',
          alignItems: 'center',
          width: '100%',
          py: 0.5,
          backgroundColor:
            selectedNode && node.path === selectedNode
              ? 'lightgray'
              : isDragOver
              ? 'lightgreen'
              : 'inherit',
          cursor: 'pointer',
        }}
      >
        <IconButton
          onClick={handleMenuClick}
          size="small"
          sx={{ marginRight: 1 }}
        >
          <MoreVertIcon />
        </IconButton>

        <Menu anchorEl={menuAnchorEl} open={Boolean(menuAnchorEl)} onClose={handleCloseMenu}>
          {menuItems.map((mi) => (
            <MenuItem
              key={mi}
              onClick={(event) => {
                event.stopPropagation();
                handleMenuAction(mi.toLowerCase().replaceAll(' ', '-'));
              }}
            >
              {mi}
            </MenuItem>
          ))}
        </Menu>

        {!isRoot && (
          <>
            {node.childrenCount > 0 && (
              <IconButton
                onClick={(e) => {
                  e.stopPropagation();
                  onToggleExpand(nodePathKey);
                }}
                size="small"
              >
                {open ? <ExpandLessIcon /> : <ExpandMoreIcon />}
              </IconButton>
            )}
            <ListItemIcon>
              {evaluating ? <CircularProgress size="1rem" /> : getIconForExpressionType()}
            </ListItemIcon>
            {!renameMode ? (
              <ListItemText primary={node.name} />
            ) : (
              <TextField
                size="small"
                value={renamedName}
                onChange={(e) => onStateChange(nodePathKey, { renamedName: e.target.value })}
                autoFocus
                onKeyDown={(e) => {
                  if (e.key === 'Enter') onApplyRename(node);
                  else if (e.key === 'Escape') onStateChange(nodePathKey, { renameMode: false });
                }}
              />
            )}
          </>
        )}
        {isRoot && <ListItemText primary="Nodes" />}
      </ListItem>

      {/* If user clicked "add" -> show input field */}
      {newInputMode && (
        <Box pl={isRoot ? 0 : 4}>
          <TextField
            size="small"
            label="Enter name"
            value={newName}
            onChange={(e) => onStateChange(nodePathKey, { newName: e.target.value })}
            onKeyDown={(e) => {
              if (e.key === 'Enter') onAddItem(node);
              else if (e.key === 'Escape') onStateChange(nodePathKey, { newInputMode: false });
            }}
            autoFocus
          />
        </Box>
      )}

      {/* Collapsed children */}
      {open && (
        <Collapse in={open} timeout="auto" unmountOnExit>
          <List component="div" disablePadding dense>
            {children?.map((child: NodeItem, index: number) => (
              <Box key={index} pl={isRoot ? 0 : 4}>
                <EvalNodeComponent
                  node={child}
                  nodeStates={nodeStates}
                  onStateChange={onStateChange}
                  onSelect={onSelect}
                  onToggleExpand={onToggleExpand}
                  onApplyRename={onApplyRename}
                  onDeleteItem={onDeleteItem}
                  onAddItem={onAddItem}
                  onDragStart={onDragStart}
                  onDragOver={onDragOver}
                  onDragLeave={onDragLeave}
                  onDrop={onDrop}
                  selectedNode={selectedNode}
                />
              </Box>
            ))}
          </List>
        </Collapse>
      )}
    </>
  );
};

export default EvalNodeComponent;
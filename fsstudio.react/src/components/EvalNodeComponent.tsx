// EvalNodeComponent.tsx
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
  sessionId: string;
  nodeStates: Record<string, any>;
  onStateChange: (nodePath: string, updatedState: Partial<any>) => void;
  onFetchChildren: (path: string | null) => void;
  onSelect: (node: NodeItem) => void;
  onToggleExpand: (nodePath: string) => void;
  onMenuClick: (nodePath: string, anchorEl: HTMLElement) => void;
  onCloseMenu: (nodePath: string) => void;
  onMenuAction: (node: NodeItem, action: string) => void;
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
  sessionId,
  nodeStates,
  onStateChange,
  onFetchChildren,
  onSelect,
  onToggleExpand,
  onMenuClick,
  onCloseMenu,
  onMenuAction,
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
    menuAnchorEl,
    renameMode,
    renamedName,
    newInputMode,
    newName,
    newNodeType,
    dragOver: isDragOver,
    children,
  } = nodeState;

  function getIconForExpressionType() {
    switch (node.expressionType) {
      case ExpressionType.ClearText:
        return <DescriptionIcon />;
      case ExpressionType.FuncScript:
        return <FunctionsIcon />;
      case ExpressionType.FuncScriptTextTemplate:
        return (
          <>
            <FunctionsIcon />
            <DescriptionIcon />
          </>
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
        onDragStart={(e) => onDragStart(node.path || '', e)}
        onDragOver={(e) => onDragOver(node.path || 'root', e)}
        onDragLeave={() => onDragLeave(node.path || 'root')}
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
          '&:hover':
            node.childrenCount === 0
              ? {
                  backgroundColor: 'lightblue',
                }
              : {},
        }}
      >
        <IconButton
          onClick={(e) => {
            onMenuClick(node.path || 'root', e.currentTarget);
            e.stopPropagation();
          }}
          size="small"
        >
          <MoreVertIcon />
        </IconButton>

        <Menu
          anchorEl={menuAnchorEl}
          open={Boolean(menuAnchorEl)}
          onClose={() => onCloseMenu(node.path || 'root')}
        >
          {menuItems.map((mi) => (
            <MenuItem
              key={mi}
              onClick={(event) => {
                event.stopPropagation();
                onMenuAction(node, mi.toLowerCase().replaceAll(' ', '-'));
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
                  onToggleExpand(node.path || 'root');
                }}
                size="small"
              >
                {open ? <ExpandLessIcon /> : <ExpandMoreIcon />}
              </IconButton>
            )}
            <ListItemIcon>{getIconForExpressionType()}</ListItemIcon>
            {!renameMode ? (
              <ListItemText primary={node.name} />
            ) : (
              <TextField
                size="small"
                value={renamedName}
                onChange={(e) =>
                  onStateChange(nodePathKey, { renamedName: e.target.value })
                }
                autoFocus
                onKeyDown={(e) => {
                  if (e.key === 'Enter') onApplyRename(node);
                  else if (e.key === 'Escape')
                    onStateChange(nodePathKey, { renameMode: false });
                }}
              />
            )}
          </>
        )}
        {isRoot && <ListItemText primary="Nodes" />}
      </ListItem>

      {newInputMode && (
        <Box pl={isRoot ? 0 : 4}>
          <TextField
            size="small"
            label="Enter name"
            value={newName}
            onChange={(e) => onStateChange(nodePathKey, { newName: e.target.value })}
            onKeyDown={(e) => {
              if (e.key === 'Enter') onAddItem(node);
              else if (e.key === 'Escape')
                onStateChange(nodePathKey, { newInputMode: false });
            }}
            autoFocus
          />
        </Box>
      )}

      {open && (
        <Collapse in={open} timeout="auto" unmountOnExit>
          <List component="div" disablePadding dense>
            {children?.map((child: NodeItem, index: number) => (
              <Box key={index} pl={isRoot ? 0 : 4}>
                <EvalNodeComponent
                  node={child}
                  sessionId={sessionId}
                  nodeStates={nodeStates}
                  onStateChange={onStateChange}
                  onFetchChildren={onFetchChildren}
                  onSelect={onSelect}
                  onToggleExpand={onToggleExpand}
                  onMenuClick={onMenuClick}
                  onCloseMenu={onCloseMenu}
                  onMenuAction={onMenuAction}
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
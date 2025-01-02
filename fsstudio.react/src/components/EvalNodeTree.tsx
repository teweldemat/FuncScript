// EvalNodeTree.tsx
import React, { useEffect, useCallback } from 'react';
import { Box, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button, List } from '@mui/material';
import EvalNodeComponent from './EvalNodeComponent';
import { ExpressionType, NodeItem, useFsStudio } from '../FsStudioProvider';

export const EvalNodeTree: React.FC<{
  rootNode: NodeItem;
  sessionId: string;
  onSelect: (path: string | null) => void;
  selectedNode: string | null;
}> = ({ rootNode, sessionId, onSelect, selectedNode }) => {
  const {
    sessions,
    setSessions,
    fetchChildren,
    renameNode,
    deleteNode,
    createNode,
    moveNode,
    updateNodeStates,
  } = useFsStudio();

  // We can store everything in sessions[sessionId], falling back if not present
  const sessionData = sessions[sessionId] || {
    nodeStates: {},
    selectedNode: null,
  };

  useEffect(() => {
    initializeNodeState(rootNode);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rootNode]);

  const initializeNodeState = (node: NodeItem) => {
    setSessions((prev) => {
      const existingSession = prev[sessionId] || { nodeStates: {}, selectedNode: null };
      if (!existingSession.nodeStates[node.path || 'root']) {
        existingSession.nodeStates[node.path || 'root'] = {
          open: !node.path,
          menuAnchorEl: null,
          renameMode: false,
          renamedName: node.name,
          newInputMode: false,
          newName: '',
          newNodeType: ExpressionType.FuncScript,
          deleteItem: false,
          dragOver: false,
          children: [],
        };
      }
      return { ...prev, [sessionId]: existingSession };
    });
    fetchChildren(sessionId, node.path).then((chs) => {
      updateNodeStates(sessionId, node.path || 'root', { children: chs });
    });
  };

  const fetchChildNodes = useCallback(
    (path: string | null) => {
      const key = path || 'root';
      fetchChildren(sessionId, path).then((chs) => {
        updateNodeStates(sessionId, key, { children: chs });
      });
    },
    [fetchChildren, sessionId, updateNodeStates]
  );

  const handleStateChange = (nodePath: string, updatedState: Partial<any>) => {
    updateNodeStates(sessionId, nodePath, updatedState);
  };

  const handleMenuClick = (nodePath: string, anchorEl: HTMLElement) => {
    const existing = sessionData.nodeStates[nodePath] || {};
    handleStateChange(nodePath, { ...existing, menuAnchorEl: anchorEl });
  };

  const handleCloseMenu = (nodePath: string) => {
    handleStateChange(nodePath, { menuAnchorEl: null });
  };

  const handleMenuAction = (node: NodeItem, action: string) => {
    const nodePath = node.path || 'root';
    handleCloseMenu(nodePath);
    switch (action) {
      case 'add-standard':
      case 'add-text':
      case 'add-text-template':
        handleStateChange(nodePath, {
          newInputMode: true,
          newNodeType:
            action === 'add-standard'
              ? ExpressionType.FuncScript
              : action === 'add-text'
              ? ExpressionType.ClearText
              : ExpressionType.FuncScriptTextTemplate,
        });
        break;
      case 'rename':
        handleStateChange(nodePath, { renameMode: true, renamedName: node.name });
        break;
      case 'delete':
        handleStateChange(nodePath, { deleteItem: true });
        break;
      default:
        break;
    }
  };

  const handleToggleExpand = (nodePath: string) => {
    const nodeState = sessionData.nodeStates[nodePath] || {};
    const newOpen = !nodeState.open;
    handleStateChange(nodePath, { open: newOpen });
    if (newOpen) {
      fetchChildNodes(nodePath === 'root' ? null : nodePath);
    }
  };

  const handleSelect = (node: NodeItem) => {
    onSelect(node.path || null);
  };

  const handleApplyRename = async (node: NodeItem) => {
    const nodePath = node.path || 'root';
    const nameToSet = sessionData.nodeStates[nodePath].renamedName;
    try {
      await renameNode(sessionId, nodePath, nameToSet);
      handleStateChange(nodePath, { renameMode: false });
      fetchChildNodes(getParentPath(nodePath));
    } catch (error) {
      console.error('Error updating expression:', error);
    }
  };

  const handleDeleteItem = async (node: NodeItem) => {
    const nodePath = node.path || 'root';
    try {
      await deleteNode(sessionId, nodePath);
      if (selectedNode === node.path) {
        onSelect(null);
      }
      handleStateChange(nodePath, { deleteItem: false });
      fetchChildNodes(getParentPath(nodePath));
    } catch (error) {
      console.error('Error deleting node:', error);
    }
  };

  const handleAddItem = async (node: NodeItem) => {
    const nodePath = node.path || 'root';
    const { newName, newNodeType } = sessionData.nodeStates[nodePath];
    if (newName.trim() !== '') {
      try {
        await createNode(sessionId, node.path, newName, newNodeType);
        handleStateChange(nodePath, { newInputMode: false, newName: '' });
        onSelect(node.path ? `${node.path}.${newName}` : newName);
        handleStateChange(nodePath, { open: true });
        fetchChildNodes(node.path);
      } catch (error) {
        console.error('Error creating item:', error);
        alert('Failed to create item: ' + (error as Error).message);
      }
    }
  };

  const handleDragStart = (draggedNodePath: string, e: React.DragEvent) => {
    e.stopPropagation();
    e.dataTransfer.setData('draggedPath', draggedNodePath);
    if (draggedNodePath) {
      const lastDotIndex = draggedNodePath.lastIndexOf('.');
      if (lastDotIndex > -1) {
        const oldParentPath = draggedNodePath.substring(0, lastDotIndex);
        e.dataTransfer.setData('oldParentPath', oldParentPath);
      } else {
        e.dataTransfer.setData('oldParentPath', '');
      }
    }
  };

  const handleDragOver = (nodePath: string, e: React.DragEvent) => {
    e.preventDefault();
    handleStateChange(nodePath, { dragOver: true });
  };

  const handleDragLeave = (nodePath: string) => {
    handleStateChange(nodePath, { dragOver: false });
  };

  const handleDrop = async (node: NodeItem, e: React.DragEvent) => {
    e.preventDefault();
    const nodePath = node.path || 'root';
    handleStateChange(nodePath, { dragOver: false });
    const draggedPath = e.dataTransfer.getData('draggedPath');
    const oldParentPath = e.dataTransfer.getData('oldParentPath');
    if (draggedPath && draggedPath !== node.path) {
      try {
        await moveNode(sessionId, draggedPath, node.path || null);
        fetchChildNodes(oldParentPath || null);
        fetchChildNodes(node.path || null);
      } catch (error) {
        console.error('Error moving node:', error);
      }
    }
  };

  const getParentPath = (path: string) => {
    const idx = path.lastIndexOf('.');
    return idx > 0 ? path.slice(0, idx) : null;
  };

  return (
    <Box>
      <EvalNodeComponent
        node={rootNode}
        sessionId={sessionId}
        selectedNode={selectedNode}
        nodeStates={sessionData.nodeStates}
        onStateChange={handleStateChange}
        onFetchChildren={fetchChildNodes}
        onSelect={handleSelect}
        onToggleExpand={handleToggleExpand}
        onMenuClick={handleMenuClick}
        onCloseMenu={handleCloseMenu}
        onMenuAction={handleMenuAction}
        onApplyRename={handleApplyRename}
        onDeleteItem={handleDeleteItem}
        onAddItem={handleAddItem}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      />

      {Object.entries(sessionData.nodeStates).map(([path, state]) => (
        <Dialog
          key={path}
          open={state.deleteItem}
          onClose={() => handleStateChange(path, { deleteItem: false })}
        >
          <DialogTitle>Confirm Deletion</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Are you sure you want to delete {state.renamedName}? This action cannot
              be undone.
            </DialogContentText>
          </DialogContent>
          <DialogActions>
            <Button
              onClick={() => handleStateChange(path, { deleteItem: false })}
              color="primary"
            >
              Cancel
            </Button>
            <Button
              onClick={() => {
                handleDeleteItem({
                  path: path === 'root' ? '' : path,
                  name: state.renamedName,
                  childrenCount: state.children.length,
                  expressionType: ExpressionType.FsStudioParentNode,
                  expression: null,
                });
              }}
              color="primary"
              autoFocus
            >
              Confirm
            </Button>
          </DialogActions>
        </Dialog>
      ))}
    </Box>
  );
};
import React, { useEffect, useCallback, useState } from 'react';
import { Box, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from '@mui/material';
import { ExpressionType, NodeItem } from '../FsStudioProvider'; 
import EvalNodeComponent from './EvalNodeComponent';
import axios from 'axios';
import { SERVER_URL } from '../backend';

interface EvalNodeTreeProps {
  rootNode: NodeItem;
  sessionId: string;
  onSelect: (path: string | null) => void;
  selectedNode: string | null;
}

export const EvalNodeTree: React.FC<EvalNodeTreeProps> = ({
  rootNode,
  sessionId,
  onSelect,
  selectedNode,
}) => {
  const [nodeStates, setNodeStates] = useState<Record<string, any>>({});

  useEffect(() => {
    // Initialize root node state
    initializeNodeState(rootNode);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [rootNode]);

  const initializeNodeState = (node: NodeItem) => {
    setNodeStates((prev) => {
      const key = node.path || 'root';
      if (!prev[key]) {
        prev[key] = {
          open: !node.path,
          renameMode: false,
          renamedName: node.name,
          newInputMode: false,
          newName: '',
          deleteItem: false,
          dragOver: false,
          children: [],
          evaluating: false,
        };
      }
      return { ...prev };
    });
    fetchChildNodes(node.path);
  };

  const fetchChildNodes = useCallback(
    async (path: string | null) => {
      const key = path || 'root';
      try {
        const response = await axios.get<NodeItem[]>(
          `${SERVER_URL}/api/sessions/${sessionId}/node/children`,
          {
            params: { nodePath: path },
          }
        );
        const children = response.data.map((item) => ({
          ...item,
          path: path ? `${path}.${item.name}` : item.name,
        }));
        setNodeStates((prev) => ({
          ...prev,
          [key]: { ...prev[key], children },
        }));
      } catch (error) {
        console.error('Error fetching children:', error);
      }
    },
    [sessionId]
  );

  const getParentPath = (path: string) => {
    const idx = path.lastIndexOf('.');
    return idx > 0 ? path.slice(0, idx) : null;
  };

  const handleStateChange = (nodePath: string, updatedState: Partial<any>) => {
    setNodeStates((prev) => ({
      ...prev,
      [nodePath]: { ...prev[nodePath], ...updatedState },
    }));
  };

  const handleToggleExpand = (nodePath: string) => {
    const nodeState = nodeStates[nodePath] || {};
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
    const nameToSet = nodeStates[nodePath].renamedName;
    try {
      await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/rename`, {
        nodePath,
        newName: nameToSet,
      });
      handleStateChange(nodePath, { renameMode: false });
      fetchChildNodes(getParentPath(nodePath) || null);
    } catch (error) {
      console.error('Error renaming node:', error);
    }
  };

  const handleDeleteItem = async (node: NodeItem) => {
    const nodePath = node.path || 'root';
    try {
      await axios.delete(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
        params: { nodePath },
      });
      if (selectedNode === node.path) {
        onSelect(null);
      }
      handleStateChange(nodePath, { deleteItem: false });
      fetchChildNodes(getParentPath(nodePath) || null);
    } catch (error) {
      console.error('Error deleting node:', error);
    }
  };

  const handleAddItem = async (node: NodeItem) => {
    const nodePath = node.path || 'root';
    const { newName } = nodeStates[nodePath];
    if (newName.trim() !== '') {
      try {
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node`, {
          ParentNodePath: node.path,
          Name: newName,
          ExpressionType: ExpressionType.FuncScript,
          Expression: '',
        });
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
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/move`, {
          nodePath: draggedPath,
          newParentPath: node.path || null,
        });
        fetchChildNodes(oldParentPath || null);
        fetchChildNodes(node.path || null);
      } catch (error) {
        console.error('Error moving node:', error);
      }
    }
  };

  return (
    <Box>
      <EvalNodeComponent
        node={rootNode}
        nodeStates={nodeStates}
        onStateChange={handleStateChange}
        onSelect={handleSelect}
        onToggleExpand={handleToggleExpand}
        onApplyRename={handleApplyRename}
        onDeleteItem={handleDeleteItem}
        onAddItem={handleAddItem}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        selectedNode={selectedNode}
      />

      {/* All "deleteItem" dialogs for each node path */}
      {Object.entries(nodeStates).map(([path, state]) => (
        <Dialog
          key={path}
          open={Boolean(state.deleteItem)}
          onClose={() => handleStateChange(path, { deleteItem: false })}
        >
          <DialogTitle>Confirm Deletion</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Are you sure you want to delete {state.renamedName}? This action cannot be undone.
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
                  expressionType: ExpressionType.FsStudioParentNode,
                  childrenCount: state.children?.length || 0,
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
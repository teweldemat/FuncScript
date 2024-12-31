import React, { useState, useEffect, useRef } from 'react';
import { Box, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button, Menu, MenuItem, TextField, List, Collapse } from '@mui/material';
import { ExpressionType, NodeItem, useEvalNod } from './EvalNodeProvider';
import EvalNodeComponent from './EvalNodeComponent';

interface NodeState {
    open: boolean;
    menuAnchorEl: HTMLElement | null;
    renameMode: boolean;
    renamedName: string;
    newInputMode: boolean;
    newName: string;
    newNodeType: ExpressionType;
    deleteItem: boolean;
    dragOver: boolean;
    children: NodeItem[];
}

export const EvalNodeTree: React.FC<{
    rootNode: NodeItem;
    sessionId: string;
    onSelect: (path: string | null) => void;
    selectedNode: string | null;
}> = ({ rootNode, sessionId, onSelect, selectedNode }) => {
    const [nodeStates, setNodeStates] = useState<Record<string, NodeState>>({});
    const { fetchChildren, renameNode, deleteNode, createNode, moveNode } = useEvalNod();

    useEffect(() => {
        initializeNodeState(rootNode);
    }, [rootNode]);

    const initializeNodeState = (node: NodeItem) => {
        setNodeStates((prev) => {
            if (!prev[node.path || 'root']) {
                prev[node.path || 'root'] = {
                    open: !node.path,
                    menuAnchorEl: null,
                    renameMode: false,
                    renamedName: node.name,
                    newInputMode: false,
                    newName: '',
                    newNodeType: ExpressionType.FuncScript,
                    deleteItem: false,
                    dragOver: false,
                    children: []
                };
            }
            return { ...prev };
        });
        fetchChildren(sessionId, node.path).then((chs) => {
            setNodeStates((prev) => {
                const currentKey = node.path || 'root';
                if (prev[currentKey]) {
                    prev[currentKey].children = chs;
                }
                return { ...prev };
            });
        });
    };

    const fetchChildNodes = (path: string | null) => {
        const key = path || 'root';
        fetchChildren(sessionId, path).then((chs) => {
            setNodeStates((prev) => {
                if (prev[key]) {
                    prev[key].children = chs;
                }
                return { ...prev };
            });
        });
    };

    const handleMenuClick = (nodePath: string, anchorEl: HTMLElement) => {
        setNodeStates((prev) => {
          // If there's no nodeState yet, create a default:
          if (!prev[nodePath]) {
            prev[nodePath] = {
              open: false,
              menuAnchorEl: null,
              renameMode: false,
              renamedName: '',
              newInputMode: false,
              newName: '',
              newNodeType: ExpressionType.FuncScript,
              deleteItem: false,
              dragOver: false,
              children: [],
            };
          }
      
          prev[nodePath].menuAnchorEl = anchorEl;
          return { ...prev };
        });
      };

    const handleCloseMenu = (nodePath: string) => {
        setNodeStates((prev) => {
            if (prev[nodePath || 'root']) {
                prev[nodePath || 'root'].menuAnchorEl = null;
            }
            return { ...prev };
        });
    };

    const handleMenuAction = (node: NodeItem, action: string) => {
        const nodePath = node.path || 'root';
        handleCloseMenu(nodePath);
        switch (action) {
            case 'add-standard':
            case 'add-text':
            case 'add-text-template':
                setNodeStates((prev) => {
                    if (prev[nodePath]) {
                        prev[nodePath].newInputMode = true;
                        prev[nodePath].newNodeType =
                            action === 'add-standard'
                                ? ExpressionType.FuncScript
                                : action === 'add-text'
                                    ? ExpressionType.ClearText
                                    : ExpressionType.FuncScriptTextTemplate;
                    }
                    return { ...prev };
                });
                break;
            case 'rename':
                setNodeStates((prev) => {
                    if (prev[nodePath]) {
                        prev[nodePath].renameMode = true;
                        prev[nodePath].renamedName = node.name;
                    }
                    return { ...prev };
                });
                break;
            case 'delete':
                setNodeStates((prev) => {
                    if (prev[nodePath]) {
                        prev[nodePath].deleteItem = true;
                    }
                    return { ...prev };
                });
                break;
            default:
                break;
        }
    };

    const handleToggleExpand = (nodePath: string) => {
        setNodeStates((prev) => {
          // If the node state for this path doesn't exist, set a default
          if (!prev[nodePath]) {
            prev[nodePath] = {
              open: false,
              menuAnchorEl: null,
              renameMode: false,
              renamedName: '',
              newInputMode: false,
              newName: '',
              newNodeType: ExpressionType.FuncScript,
              deleteItem: false,
              dragOver: false,
              children: [],
            };
          }
          const newOpen = !prev[nodePath].open;
          prev[nodePath].open = newOpen;
          if (newOpen) {
            fetchChildNodes(nodePath === 'root' ? null : nodePath);
          }
          return { ...prev };
        });
      };

    const handleSelect = (node: NodeItem) => {
        onSelect(node.path || null);
    };
    const handleApplyRename = async (node: NodeItem) => {
        const nodePath = node.path || 'root';
        const nameToSet = nodeStates[nodePath].renamedName;
        try {
            await renameNode(sessionId, nodePath, nameToSet);
            setNodeStates((prev) => {
                if (prev[nodePath]) {
                    prev[nodePath].renameMode = false;
                }
                return { ...prev };
            });
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
            setNodeStates((prev) => {
                if (prev[nodePath]) {
                    prev[nodePath].deleteItem = false;
                }
                return { ...prev };
            });
            fetchChildNodes(getParentPath(nodePath));
        } catch (error) {
            console.error('Error deleting node:', error);
        }
    };

    const handleAddItem = async (node: NodeItem) => {
        const nodePath = node.path || 'root';
        const { newName, newNodeType } = nodeStates[nodePath];
        if (newName.trim() !== '') {
            try {
                await createNode(sessionId, node.path, newName, newNodeType);
                setNodeStates((prev) => {
                    if (prev[nodePath]) {
                        prev[nodePath].newInputMode = false;
                        prev[nodePath].newName = '';
                    }
                    return { ...prev };
                });
                onSelect(node.path ? `${node.path}.${newName}` : newName);
                setNodeStates((prev) => {
                    if (prev[nodePath]) {
                        prev[nodePath].open = true;
                    }
                    return { ...prev };
                });
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
        setNodeStates((prev) => {
            if (prev[nodePath]) {
                prev[nodePath].dragOver = true;
            }
            return { ...prev };
        });
    };

    const handleDragLeave = (nodePath: string) => {
        setNodeStates((prev) => {
            if (prev[nodePath]) {
                prev[nodePath].dragOver = false;
            }
            return { ...prev };
        });
    };

    const handleDrop = async (node: NodeItem, e: React.DragEvent) => {
        e.preventDefault();
        const nodePath = node.path || 'root';
        setNodeStates((prev) => {
            if (prev[nodePath]) {
                prev[nodePath].dragOver = false;
            }
            return { ...prev };
        });
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

    const handleStateChange = (
        nodePath: string,
        updatedState: Partial<NodeState>
    ) => {
        setNodeStates((prev) => {
            const newNodeState = { ...prev[nodePath || 'root'], ...updatedState };
            return { ...prev, [nodePath || 'root']: newNodeState };
        });
    };

    return (
        <Box>
            <EvalNodeComponent
                node={rootNode}
                sessionId={sessionId}
                selectedNode={selectedNode}
                nodeStates={nodeStates}
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

            {Object.entries(nodeStates).map(([path, state]) => (
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
                                // We need the node to match the path for the delete action
                                // so let's build a temp NodeItem
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
import React, { useState, useEffect, useRef } from 'react';
import { ListItem, ListItemIcon, ListItemText, Collapse, List, Box, IconButton, Menu, MenuItem, TextField, InputBaseComponentProps, Dialog, DialogTitle, Button, DialogActions, DialogContent, DialogContentText } from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import FolderIcon from '@mui/icons-material/Folder';
import FileCopyIcon from '@mui/icons-material/FileCopy';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import axios from 'axios';
import { SERVER_URL } from '../backend';

interface NavItem {
    name: string;
    isFolder: boolean;
    path: string;
}

interface NavItemComponentProps {
    item: NavItem;
    selectedPath?: string;
    onSelect: (path: string) => void;
    onRename: () => void;
    onDelete: () => void;
}

const NavItemComponent: React.FC<NavItemComponentProps> = ({ item, onSelect, selectedPath, onRename, onDelete }) => {
    const [children, setChildren] = useState<NavItem[]>([]);
    const [open, setOpen] = useState(item.path === "/");
    const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null);
    const [newInputMode, setNewInputMode] = useState(false);
    const [inputType, setInputType] = useState('');
    const [newName, setNewName] = useState('');
    const [renameMode, setRenameMode] = useState<boolean>(false);
    const [renameValue, setRenameValue] = useState<string>('');
    const [deleteItem, setDeleteItem] = useState<boolean>(false);
    const [duplicateMode, setDuplicateMode] = useState<boolean>(false);
    const [duplicateValue, setDuplicateValue] = useState<string>('');

    const renameInputRef = useRef<InputBaseComponentProps['inputRef']>(null);
    const newInputRef = useRef<InputBaseComponentProps['inputRef']>(null);
    const duplicateRef = useRef<InputBaseComponentProps['inputRef']>(null);

    useEffect(() => {
        if (renameMode && renameInputRef.current) {
            renameInputRef.current.focus();
            renameInputRef.current.select();
        }
    }, [renameMode]);

    useEffect(() => {
        if (newInputMode && newInputRef.current) {
            newInputRef.current.focus();
            newInputRef.current.select();
        }
    }, [newInputMode]);

    useEffect(() => {
        if (duplicateMode && duplicateRef.current) {
            duplicateRef.current.focus();
            duplicateRef.current.select();
        }
    }, [duplicateMode]);

    useEffect(() => {
        if (item.isFolder && open) {
            fetchChildren();
        }
    }, [item, open]);

    const fetchChildren = () => {
        axios.get(`${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles`, { params: { path: item.path } })
            .then(response => {
                const directories = response.data.directories.map((name: string) => ({ name, isFolder: true, path: `${item.path}${name}/` }));
                const files = response.data.files.map((name: string) => ({ name, isFolder: false, path: `${item.path}${name}` }));
                setChildren([...directories, ...files]);
            })
            .catch(error => console.error('Failed to fetch child items:', error));
    };

    const handleToggleExpand = (e: React.MouseEvent) => {
        e.stopPropagation();
        setOpen(!open);
    };

    const handleSelect = (e: React.MouseEvent) => {
        e.stopPropagation();
        onSelect(item.path);
    };

    const handleMenuClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        setMenuAnchorEl(event.currentTarget);
    };

    const handleCloseMenu = () => {
        setMenuAnchorEl(null);
    };

    const handleMenuAction = (action: string) => {
        handleCloseMenu();
        switch (action) {
            case 'add-file':
            case 'add-folder':
                setNewInputMode(true);
                setInputType(action === 'add-file' ? 'file' : 'folder');
                break;
            case 'rename':
                setRenameMode(true);
                setRenameValue(item.name);
                break;
            case 'delete':
                setDeleteItem(true);
                break;
            case 'duplicate':
                setDuplicateMode(true);
                setDuplicateValue(item.name + '_copy');
                break;
            default:
                break;
        }
    };

    const handleAddItem = () => {
        if (newName.trim() !== '') {
            const apiPath = inputType === 'file' ? '/CreateFile' : '/CreateFolder';
            axios.post(`${SERVER_URL}/api/FileSystem${apiPath}`, {
                Path: item.path,
                Name: newName
            })
                .then(response => {
                    fetchChildren();
                    setNewName('');
                    setNewInputMode(false);
                })
                .catch(error => {
                    alert('Failed to create item: ' + error.message);
                });
        }
    };

    const handleRenameItem = () => {
        if (renameValue.trim() !== '') {
            axios.put(`${SERVER_URL}/api/FileSystem/RenameItem`, {
                Path: item.path,
                Name: renameValue
            })
                .then(response => {
                    onRename();
                    setRenameMode(false);
                    setRenameValue('');
                })
                .catch(error => {
                    alert('Failed to rename item: ' + error.message);
                });
        }
    };

    const handleDeleteItem = () => {
        axios.delete(`${SERVER_URL}/api/FileSystem/DeleteItem`, {
            params: {
                Path: item.path,
            }
        })
            .then(response => {
                setDeleteItem(false);
                onDelete();
            })
            .catch(error => {
                alert(`Failed to delete item: ${error.message}`);
            });
    };

    const handleDuplicateItem = () => {
        if (duplicateValue.trim() !== '') {
            axios.post(`${SERVER_URL}/api/FileSystem/DuplicateFile`, {
                Path: item.path,
                Name: duplicateValue
            })
                .then(response => {
                    setDuplicateMode(false);
                    setDuplicateValue('');
                    onDelete();
                })
                .catch(error => {
                    alert('Failed to duplicate item: ' + error.message);
                });
        }
    };

    const isRoot = item.path === "/";
    const menuOptions = item.isFolder
        ? (isRoot ? ['Add Folder', 'Add File'] : ['Add Folder', 'Add File', 'Rename', 'Delete'])
        : ['Rename', 'Duplicate', 'Delete'];

    return (
        <>
            <ListItem
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    width: '100%',
                    backgroundColor: item.path === selectedPath ? 'lightgray' : 'inherit',
                    cursor: 'pointer',
                    '&:hover': { backgroundColor: 'lightblue' }
                }}
                onClick={item.isFolder ? () => {} : handleSelect}
            >
                <IconButton onClick={handleMenuClick} size="small">
                    <MoreVertIcon />
                </IconButton>
                <Menu
                    anchorEl={menuAnchorEl}
                    open={Boolean(menuAnchorEl)}
                    onClose={handleCloseMenu}
                >
                    {menuOptions.map(option => (
                        <MenuItem
                            key={option}
                            onClick={() => handleMenuAction(option.toLowerCase().replace(' ', '-'))}
                        >
                            {option}
                        </MenuItem>
                    ))}
                </Menu>
                {!isRoot && (
                    <>
                        {item.isFolder && (
                            <IconButton onClick={handleToggleExpand} size="small" sx={{ marginRight: '8px' }}>
                                {open ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                            </IconButton>
                        )}
                        <ListItemIcon>
                            {item.isFolder ? <FolderIcon /> : <FileCopyIcon />}
                        </ListItemIcon>
                        {renameMode ? (
                            <TextField
                                size="small"
                                label={`Enter name`}
                                value={renameValue}
                                onChange={(e) => setRenameValue(e.target.value)}
                                onKeyDown={(e) => {
                                    if (e.key === 'Enter') handleRenameItem();
                                    else if (e.key === "Escape") setRenameMode(false);
                                }}
                                autoFocus
                                InputProps={{
                                    onFocus: event => event.target.select()
                                }}
                                inputRef={renameInputRef}
                            />
                        ) : (
                            <ListItemText primary={item.name} sx={{ flexGrow: 1 }} />
                        )}
                    </>
                )}
                {isRoot && (<ListItemText primary="Files" />)}
            </ListItem>

            {newInputMode && (
                <Box pl={isRoot ? 0 : 4}>
                    <TextField
                        size="small"
                        label={`Enter ${inputType} name`}
                        value={newName}
                        onChange={(e) => setNewName(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === 'Enter') handleAddItem();
                            else if (e.key === "Escape") setNewInputMode(false);
                        }}
                        autoFocus
                        inputRef={newInputRef}
                    />
                </Box>
            )}
            {item.isFolder && (
                <Collapse in={open} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                        {children.map((childItem, index) => (
                            <Box key={index} sx={{ pl: isRoot ? 0 : 4 }}>
                                <NavItemComponent
                                    item={childItem}
                                    selectedPath={selectedPath}
                                    onSelect={onSelect}
                                    onDelete={() => fetchChildren()}
                                    onRename={() => fetchChildren()}
                                />
                            </Box>
                        ))}
                    </List>
                </Collapse>
            )}

            <Dialog
                open={deleteItem}
                onClose={() => setDeleteItem(false)}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{"Confirm Deletion"}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        Are you sure you want to delete {item.name}? This action cannot be undone.
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

            <Dialog
                open={duplicateMode}
                onClose={() => setDuplicateMode(false)}
                aria-labelledby="duplicate-dialog-title"
                aria-describedby="duplicate-dialog-description"
            >
                <DialogTitle id="duplicate-dialog-title">Duplicate File</DialogTitle>
                <DialogContent>
                    <DialogContentText id="duplicate-dialog-description">
                        Enter the new name for your duplicated file
                    </DialogContentText>
                    <TextField
                        size="small"
                        value={duplicateValue}
                        onChange={(e) => setDuplicateValue(e.target.value)}
                        onKeyDown={(e) => {
                            if (e.key === 'Enter') handleDuplicateItem();
                            else if (e.key === "Escape") setDuplicateMode(false);
                        }}
                        autoFocus
                        inputRef={duplicateRef}
                        fullWidth
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDuplicateMode(false)} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={handleDuplicateItem} color="primary" autoFocus>
                        Confirm
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default NavItemComponent;
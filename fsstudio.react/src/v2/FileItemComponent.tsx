import React, { useState, MouseEvent } from 'react';
import {
    ListItem,
    ListItemIcon,
    ListItemText,
    Collapse,
    List,
    Box,
    IconButton,
    Tooltip,
    Dialog,
    DialogTitle,
    Button,
    DialogActions,
    DialogContent,
    DialogContentText,
    TextField,
    Menu,
    MenuItem,
} from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import FolderIcon from '@mui/icons-material/Folder';
import FileCopyIcon from '@mui/icons-material/FileCopy';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import CreateNewFolderIcon from '@mui/icons-material/CreateNewFolder';
import NoteAddIcon from '@mui/icons-material/NoteAdd';
import FolderOpenIcon from '@mui/icons-material/FolderOpen';
import { FileNode } from './FileTree';

interface FileItemProps {
    fileNode: FileNode;
    selectedPath: string;
    onSelect: (path: string) => void;
    onToggleExpand: (path: string) => void;
    onCreate: (path: string, name: string, type: 'folder' | 'file') => void;
    onDuplicate: (path: string, newName: string) => void;
    onDelete: (path: string) => void;
    onRename: (path: string, newName: string) => void;
}

const FileItemComponent: React.FC<FileItemProps> = ({
    fileNode,
    selectedPath,
    onSelect,
    onToggleExpand,
    onCreate,
    onDuplicate,
    onDelete,
    onRename,
}) => {
    const [newInputMode, setNewInputMode] = useState(false);
    const [inputType, setInputType] = useState<'folder' | 'file'>('folder');
    const [newName, setNewName] = useState('');
    const [renameMode, setRenameMode] = useState(false);
    const [renameValue, setRenameValue] = useState('');
    const [deleteItem, setDeleteItem] = useState(false);
    const [duplicateMode, setDuplicateMode] = useState(false);
    const [duplicateValue, setDuplicateValue] = useState('');
    const [menuAnchor, setMenuAnchor] = useState<null | HTMLElement>(null);

    const isRoot = fileNode.path === '/';

    const handleRootAction = (action: 'open-folder' | 'add-folder' | 'add-file') => {
        switch (action) {
            case 'open-folder':
                window.location.href = '/open-folder-dialog';
                break;
            case 'add-folder':
                setNewInputMode(true);
                setInputType('folder');
                break;
            case 'add-file':
                setNewInputMode(true);
                setInputType('file');
                break;
        }
    };

    const handleMenuClick = (e: MouseEvent<HTMLButtonElement>) => {
        e.stopPropagation();
        setMenuAnchor(e.currentTarget);
    };

    const handleMenuClose = () => {
        setMenuAnchor(null);
    };

    const openRenameDialog = () => {
        setRenameMode(true);
        setRenameValue(fileNode.name);
        handleMenuClose();
    };

    const openDeleteDialog = () => {
        setDeleteItem(true);
        handleMenuClose();
    };

    const openDuplicateDialog = () => {
        setDuplicateMode(true);
        setDuplicateValue(fileNode.name + '_copy');
        handleMenuClose();
    };

    return (
        <>
            {isRoot ? (
                <Box
                    sx={{
                        display: 'flex',
                        alignItems: 'center',
                        width: '100%',
                        backgroundColor: 'lightgray',
                        padding: '8px',
                    }}
                >
                    <Tooltip title="Open Folder">
                        <IconButton onClick={() => handleRootAction('open-folder')}>
                            <FolderOpenIcon />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Add Folder">
                        <IconButton onClick={() => handleRootAction('add-folder')}>
                            <CreateNewFolderIcon />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Add File">
                        <IconButton onClick={() => handleRootAction('add-file')}>
                            <NoteAddIcon />
                        </IconButton>
                    </Tooltip>
                </Box>
            ) : (
                <ListItem
                    sx={{
                        display: 'flex',
                        alignItems: 'center',
                        width: '100%',
                        backgroundColor: fileNode.path === selectedPath ? 'lightgray' : 'inherit',
                        cursor: 'pointer',
                        '&:hover': { backgroundColor: 'lightblue' },
                    }}
                    onClick={() => {
                        if (!fileNode.isFolder) onSelect(fileNode.path);
                    }}
                >
                    {!isRoot && fileNode.isFolder && (
                        <IconButton
                            onClick={(e) => {
                                e.stopPropagation();
                                onToggleExpand(fileNode.path);
                            }}
                            size="small"
                            sx={{ marginRight: '8px' }}
                        >
                            {fileNode.expanded ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                        </IconButton>
                    )}
                    <ListItemIcon>
                        {fileNode.isFolder ? <FolderIcon /> : <FileCopyIcon />}
                    </ListItemIcon>
                    {renameMode ? (
                        <TextField
                            size="small"
                            label="Enter name"
                            value={renameValue}
                            onChange={(e) => setRenameValue(e.target.value)}
                            onKeyDown={async (e) => {
                                if (e.key === 'Enter') {
                                    await onRename(fileNode.path, renameValue);
                                    setRenameMode(false);
                                } else if (e.key === 'Escape') {
                                    setRenameMode(false);
                                }
                            }}
                            autoFocus
                        />
                    ) : (
                        <ListItemText primary={fileNode.name} />
                    )}
                    {!isRoot && (
                        <IconButton size="small" onClick={handleMenuClick}>
                            <MoreVertIcon />
                        </IconButton>
                    )}
                </ListItem>
            )}

            {newInputMode && (
                <Box pl={isRoot ? 0 : 4}>
                    <TextField
                        size="small"
                        label={`Enter ${inputType} name`}
                        value={newName}
                        onChange={(e) => setNewName(e.target.value)}
                        onKeyDown={async (e) => {
                            if (e.key === 'Enter') {
                                await onCreate(fileNode.path, newName, inputType);
                                setNewInputMode(false);
                                setNewName('');
                            } else if (e.key === 'Escape') {
                                setNewInputMode(false);
                            }
                        }}
                        autoFocus
                    />
                </Box>
            )}

            {fileNode.isFolder && (
                <Collapse in={fileNode.expanded} timeout="auto" unmountOnExit>
                    <List component="div" disablePadding>
                        {fileNode.children.map((child, idx) => (
                            <Box key={idx} sx={{ pl: isRoot ? 0 : 4 }}>
                                <FileItemComponent
                                    fileNode={child}
                                    selectedPath={selectedPath}
                                    onSelect={onSelect}
                                    onToggleExpand={onToggleExpand}
                                    onCreate={onCreate}
                                    onDuplicate={onDuplicate}
                                    onDelete={onDelete}
                                    onRename={onRename}
                                />
                            </Box>
                        ))}
                    </List>
                </Collapse>
            )}

            <Menu
                anchorEl={menuAnchor}
                open={Boolean(menuAnchor)}
                onClose={handleMenuClose}
            >
                {fileNode.isFolder
                    ? [
                        <MenuItem
                            key="new-folder"
                            onClick={() => {
                                setNewInputMode(true);
                                setInputType('folder');
                                handleMenuClose();
                            }}
                        >
                            New Folder
                        </MenuItem>,
                        <MenuItem
                            key="new-file"
                            onClick={() => {
                                setNewInputMode(true);
                                setInputType('file');
                                handleMenuClose();
                            }}
                        >
                            New File
                        </MenuItem>,
                        <MenuItem key="rename" onClick={openRenameDialog}>
                            Rename
                        </MenuItem>,
                        <MenuItem key="delete" onClick={openDeleteDialog}>
                            Delete
                        </MenuItem>,
                    ]
                    : [
                        <MenuItem key="rename" onClick={openRenameDialog}>
                            Rename
                        </MenuItem>,
                        <MenuItem key="duplicate" onClick={openDuplicateDialog}>
                            Duplicate
                        </MenuItem>,
                        <MenuItem key="delete" onClick={openDeleteDialog}>
                            Delete
                        </MenuItem>,
                    ]}
            </Menu>


            <Dialog
                open={deleteItem}
                onClose={() => setDeleteItem(false)}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">{'Confirm Deletion'}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        Are you sure you want to delete {fileNode.name}? This action cannot be undone.
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDeleteItem(false)} color="primary">
                        Cancel
                    </Button>
                    <Button
                        onClick={async () => {
                            await onDelete(fileNode.path);
                            setDeleteItem(false);
                        }}
                        color="primary"
                        autoFocus
                    >
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
                        onKeyDown={async (e) => {
                            if (e.key === 'Enter') {
                                await onDuplicate(fileNode.path, duplicateValue);
                                setDuplicateMode(false);
                            } else if (e.key === 'Escape') {
                                setDuplicateMode(false);
                            }
                        }}
                        autoFocus
                        fullWidth
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setDuplicateMode(false)} color="primary">
                        Cancel
                    </Button>
                    <Button
                        onClick={async () => {
                            await onDuplicate(fileNode.path, duplicateValue);
                            setDuplicateMode(false);
                        }}
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

export default FileItemComponent;

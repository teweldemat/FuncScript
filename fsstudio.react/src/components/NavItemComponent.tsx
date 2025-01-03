import React, { useState, useEffect, useRef } from 'react';
import {
  ListItem,
  ListItemIcon,
  ListItemText,
  Collapse,
  List,
  Box,
  IconButton,
  Menu,
  MenuItem,
  TextField,
  Dialog,
  DialogTitle,
  Button,
  DialogActions,
  DialogContent,
  DialogContentText,
} from '@mui/material';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import FolderIcon from '@mui/icons-material/Folder';
import FileCopyIcon from '@mui/icons-material/FileCopy';
import ExpandLessIcon from '@mui/icons-material/ExpandLess';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
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

const NavItemComponent: React.FC<NavItemComponentProps> = ({
  item,
  onSelect,
  selectedPath,
  onRename,
  onDelete,
}) => {
  const [children, setChildren] = useState<NavItem[]>([]);
  const [open, setOpen] = useState(item.path === '/');
  const [menuAnchorEl, setMenuAnchorEl] = useState<null | HTMLElement>(null);
  const [newInputMode, setNewInputMode] = useState(false);
  const [inputType, setInputType] = useState('');
  const [newName, setNewName] = useState('');
  const [renameMode, setRenameMode] = useState<boolean>(false);
  const [renameValue, setRenameValue] = useState<string>('');
  const [deleteItem, setDeleteItem] = useState<boolean>(false);
  const [duplicateMode, setDuplicateMode] = useState<boolean>(false);
  const [duplicateValue, setDuplicateValue] = useState<string>('');

  const renameInputRef = useRef(null);
  const newInputRef = useRef(null);
  const duplicateRef = useRef(null);

  useEffect(() => {
    if (renameMode && renameInputRef.current) {
      (renameInputRef.current as HTMLInputElement).focus();
      (renameInputRef.current as HTMLInputElement).select();
    }
  }, [renameMode]);

  useEffect(() => {
    if (newInputMode && newInputRef.current) {
      (newInputRef.current as HTMLInputElement).focus();
      (newInputRef.current as HTMLInputElement).select();
    }
  }, [newInputMode]);

  useEffect(() => {
    if (duplicateMode && duplicateRef.current) {
      (duplicateRef.current as HTMLInputElement).focus();
      (duplicateRef.current as HTMLInputElement).select();
    }
  }, [duplicateMode]);

  // Stub for listing children
  const listSubFoldersAndFiles = async (path: string) => {
    try {
      const resp = await fetch(`${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles?path=${path}`);
      if (!resp.ok) throw new Error('Request failed');
      const data = await resp.json();
      return data as { directories: string[]; files: string[] };
    } catch (error) {
      console.error('Failed to fetch child items:', error);
      return { directories: [], files: [] };
    }
  };

  useEffect(() => {
    if (item.isFolder && open) {
      fetchChildren();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [item, open]);

  const fetchChildren = () => {
    listSubFoldersAndFiles(item.path)
      .then((response) => {
        const directories = response.directories.map((name: string) => ({
          name,
          isFolder: true,
          path: `${item.path}${name}/`,
        }));
        const files = response.files.map((name: string) => ({
          name,
          isFolder: false,
          path: `${item.path}${name}`,
        }));
        setChildren([...directories, ...files]);
      })
      .catch((error) => console.error('Failed to fetch child items:', error));
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
      case 'open-folder':
        window.location.href = '/open-folder-dialog';
        break;
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

  // ... the rest of your create/rename/delete/duplicate logic goes here ...

  const isRoot = item.path === '/';
  const menuOptions = item.isFolder
    ? isRoot
      ? ['Open Folder', 'Add Folder', 'Add File']
      : ['Add Folder', 'Add File', 'Rename', 'Delete']
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
          '&:hover': { backgroundColor: 'lightblue' },
        }}
        onClick={item.isFolder ? () => {} : handleSelect}
      >
        <IconButton onClick={handleMenuClick} size="small">
          <MoreVertIcon />
        </IconButton>
        <Menu anchorEl={menuAnchorEl} open={Boolean(menuAnchorEl)} onClose={handleCloseMenu}>
          {menuOptions.map((option) => (
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
            <ListItemIcon>{item.isFolder ? <FolderIcon /> : <FileCopyIcon />}</ListItemIcon>
            {renameMode ? (
              <TextField
                size="small"
                label="Enter name"
                value={renameValue}
                onChange={(e) => setRenameValue(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    // handle rename call
                  } else if (e.key === 'Escape') setRenameMode(false);
                }}
                autoFocus
                inputRef={renameInputRef}
              />
            ) : (
              <ListItemText primary={item.name} sx={{ flexGrow: 1 }} />
            )}
          </>
        )}
        {isRoot && <ListItemText primary="Files" />}
      </ListItem>

      {newInputMode && (
        <Box pl={isRoot ? 0 : 4}>
          <TextField
            size="small"
            label={`Enter ${inputType} name`}
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === 'Enter') {
                // handle add item
              } else if (e.key === 'Escape') {
                setNewInputMode(false);
              }
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

      {/* Delete and Duplicate Dialogs go here (if you want to keep the same logic) */}
      <Dialog
        open={deleteItem}
        onClose={() => setDeleteItem(false)}
        aria-labelledby="alert-dialog-title"
        aria-describedby="alert-dialog-description"
      >
        <DialogTitle id="alert-dialog-title">{'Confirm Deletion'}</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            Are you sure you want to delete {item.name}? This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteItem(false)} color="primary">
            Cancel
          </Button>
          <Button
            onClick={() => {
              // handle delete
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
            onKeyDown={(e) => {
              // handle duplicate
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
          <Button
            onClick={() => {
              // handle duplicate
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

export default NavItemComponent;
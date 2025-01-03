import React, { useState } from 'react';
import { Drawer, Box } from '@mui/material';
import FileItemComponent from './FileItemComponent';

const drawerWidth = '20%';

interface FileTreeProps {
  onSelected: (path: string) => void;
  initiallySelectedPath: string | null;
}

const FileTree: React.FC<FileTreeProps> = ({
  onSelected,
  initiallySelectedPath,
}) => {
  const [selectedPath, setSelectedPath] = useState<string>(
    initiallySelectedPath ?? ''
  );

  const handleSelect = (path: string) => {
    if (selectedPath !== path) {
      setSelectedPath(path);
      onSelected(path);
    }
  };

  return (
    <Drawer
      variant="permanent"
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
      }}
    >
      <Box sx={{ overflow: 'auto' }}>
        <FileItemComponent
          item={{ path: '/', name: 'root', isFolder: true }}
          onDelete={() => {}}
          onRename={() => {}}
          selectedPath={selectedPath}
          onSelect={handleSelect}
        />
      </Box>
    </Drawer>
  );
};

export default FileTree;

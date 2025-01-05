import React, { useState, useEffect } from 'react';
import { Box } from '@mui/material';
import FileItemComponent from './FileItemComponent';
import { useExecutionSession } from './SessionContext';

export interface FileNode {
  path: string;
  name: string;
  isFolder: boolean;
  expanded: boolean;
  children: FileNode[];
}

interface FileTreeProps {
  onSelected: (path: string) => void;
  initiallySelectedPath: string | null;
}

const FileTree: React.FC<FileTreeProps> = ({
  onSelected,
  initiallySelectedPath,
}) => {
  const [treeData, setTreeData] = useState<FileNode | null>(null);
  const [selectedPath, setSelectedPath] = useState<string>(
    initiallySelectedPath ?? ''
  );
  const {
    listDirectory,
    createFolder,
    createFile,
    duplicateFile,
    deleteItem: deleteItemFromBackend,
    renameItem,
  } = useExecutionSession() ?? {};

  const buildNode = async (path: string, name: string, isFolder: boolean) => {
    if (!listDirectory) return { path, name, isFolder, expanded: false, children: [] };
    const node: FileNode = {
      path,
      name,
      isFolder,
      expanded: path === '/',
      children: [],
    };
    if (node.isFolder) {
      try {
        const data = await listDirectory(path);
        const directories = data.directories.map((d: string) =>
          buildNode(`${path}${d}/`, d, true)
        );
        const files = data.files.map((f: string) =>
          buildNode(`${path}${f}`, f, false)
        );
        const results = await Promise.all([...directories, ...files]);
        node.children = results;
      } catch {
        node.children = [];
      }
    }
    return node;
  };

  const loadRoot = async () => {
    const root = await buildNode('/', 'root', true);
    setTreeData(root);
  };

  useEffect(() => {
    loadRoot();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSelect = (path: string) => {
    if (selectedPath !== path) {
      setSelectedPath(path);
      onSelected(path);
    }
  };

  const toggleExpand = (path: string) => {
    if (!treeData) return;
    const clone = { ...treeData };
    const toggle = (node: FileNode) => {
      if (node.path === path) {
        node.expanded = !node.expanded;
      } else {
        node.children.forEach((child) => toggle(child));
      }
    };
    toggle(clone);
    setTreeData(clone);
  };

  const refreshChildren = async (path: string) => {
    if (!treeData) return;
    const clone = { ...treeData };
    const refreshNode = async (node: FileNode) => {
      if (node.path === path) {
        const data = await listDirectory?.(node.path);
        if (data) {
          const directories = data.directories.map((d: string) =>
            buildNode(`${node.path}${d}/`, d, true)
          );
          const files = data.files.map((f: string) =>
            buildNode(`${node.path}${f}`, f, false)
          );
          const results = await Promise.all([...directories, ...files]);
          node.children = results;
        }
      } else {
        for (const child of node.children) {
          if (child.isFolder) await refreshNode(child);
        }
      }
    };
    await refreshNode(clone);
    setTreeData(clone);
  };

  const handleCreate = async (
    parentPath: string,
    name: string,
    type: 'folder' | 'file'
  ) => {
    if (type === 'folder') {
      await createFolder?.(parentPath, name);
    } else {
      await createFile?.(parentPath, name);
    }
    await refreshChildren(parentPath);
    if (type === 'file') {
      const newPath = `${parentPath}${name}`;
      setSelectedPath(newPath);
      onSelected(newPath);
    }
  };

  const handleDuplicate = async (path: string, newName: string) => {
    await duplicateFile?.(path, newName);
    const parent = path.substring(0, path.lastIndexOf('/') + 1);
    await refreshChildren(parent || '/');
    const newPath = `${parent}${newName}`;
    setSelectedPath(newPath);
    onSelected(newPath);
  };

  const handleDelete = async (path: string) => {
    await deleteItemFromBackend?.(path);
    if(path.endsWith('/'))
        path=path.substring(0,path.length-1);
    const parent = path.substring(0, path.lastIndexOf('/') + 1);
    await refreshChildren(parent || '/');
    setSelectedPath('');
    onSelected('');
  };

  const handleRename = async (path: string, newName: string) => {
    await renameItem?.(path, newName);
    const parent = path.substring(0, path.lastIndexOf('/') + 1);
    await refreshChildren(parent || '/');
  };

  return (
    <Box sx={{ overflow: 'auto' }}>
      {treeData && (
        <FileItemComponent
          fileNode={treeData}
          selectedPath={selectedPath}
          onSelect={handleSelect}
          onToggleExpand={toggleExpand}
          onCreate={handleCreate}
          onDuplicate={handleDuplicate}
          onDelete={handleDelete}
          onRename={handleRename}
        />
      )}
    </Box>
  );
};

export default FileTree;

import React, { useState } from 'react';
import FileTree from './FileTree';
import { ExecussionSessionView } from './ExecussionSessionView';

export function FileTreeExecutionView() {
  const [selectedFile, setSelectedFile] = useState('');

  return (
    <div style={{ display: 'flex' }}>
      <FileTree onSelected={setSelectedFile} initiallySelectedPath='' />
      <ExecussionSessionView selectedFile={selectedFile} />
    </div>
  );
}

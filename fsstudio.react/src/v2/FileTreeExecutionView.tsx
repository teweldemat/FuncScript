import React, { useState } from 'react';
import FileTree from './FileTree';
import { ExecussionSessionView } from './ExecussionSessionView';
import { useExecutionSession } from './ExecutionSessionProvider';

export function FileTreeExecutionView() {
  return (
    <div style={{ display: 'flex' }}>
      
      (<ExecussionSessionView />)
           
    </div>
  );
}

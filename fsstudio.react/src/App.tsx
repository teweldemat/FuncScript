import React from 'react';
import { ExecutionSessionProvider } from './v2/ExecutionSessionProvider';
import { ExecustionSimpleView } from './v2/ExecustionSimpleView';
import { FileTreeExecutionView } from './v2/FileTreeExecutionView';
import { ExecussionSessionView } from './v2/ExecussionSessionView';

 export function App() {
    return (
        <ExecutionSessionProvider>
            <ExecussionSessionView />
        </ExecutionSessionProvider>
    );
}
export default App;
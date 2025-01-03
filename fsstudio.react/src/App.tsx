import React from 'react';
import { ExecutionSessionProvider } from './v2/ExecutionSessionProvider';
import { ExecustionSimpleView } from './v2/ExecustionSimpleView';
import { FileTreeExecutionView } from './v2/FileTreeExecutionView';

 export function App() {
    return (
        <ExecutionSessionProvider>
            <FileTreeExecutionView />
        </ExecutionSessionProvider>
    );
}
export default App;
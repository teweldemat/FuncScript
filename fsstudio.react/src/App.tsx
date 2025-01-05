import React from 'react';
import { ExecutionSessionProvider, useExecutionSession } from './components/SessionContext';
import { ExecussionSessionView } from './components/ExecussionSessionView';

 export function App() {
    return (
        <ExecutionSessionProvider>
                <ExecussionSessionView />
        </ExecutionSessionProvider>
    );
}
export default App;
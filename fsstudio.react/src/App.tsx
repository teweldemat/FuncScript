import React from 'react';
import { ExecutionSessionProvider, useExecutionSession } from './v2/SessionContext';
import { ExecussionSessionView } from './v2/ExecussionSessionView';

 export function App() {
    return (
        <ExecutionSessionProvider>
                <ExecussionSessionView />
        </ExecutionSessionProvider>
    );
}
export default App;
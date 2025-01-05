import React from 'react';
import { ExecutionSessionProvider, useExecutionSession } from './v2/ExecutionSessionProvider';
import { ExecussionSessionView } from './v2/ExecussionSessionView';
import { ExecContainer } from './v2/ExecContainer';

 export function App() {
    return (
        <ExecutionSessionProvider>
                <ExecussionSessionView />
        </ExecutionSessionProvider>
    );
}
export default App;
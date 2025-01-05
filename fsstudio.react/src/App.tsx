import React from 'react';
import { ExecutionSessionProvider } from './v2/ExecutionSessionProvider';
import { ExecussionSessionView } from './v2/ExecussionSessionView';

 export function App() {
    return (
        <ExecutionSessionProvider>
            <ExecussionSessionView />
        </ExecutionSessionProvider>
    );
}
export default App;
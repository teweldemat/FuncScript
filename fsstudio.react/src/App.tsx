import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ExecutionSessionProvider } from './components/SessionContext';
import { ExecussionSessionView } from './components/ExecussionSessionView';
import { OpenFileModal } from './OpenFileModal';

export function App() {
    return (
        <Router>
            <ExecutionSessionProvider>
                <Routes>
                    <Route path="/" element={<ExecussionSessionView />} />
                    <Route path="/open-dialog-web" element={<OpenFileModal />} />
                </Routes>
            </ExecutionSessionProvider>
        </Router>
    );
}
export default App;
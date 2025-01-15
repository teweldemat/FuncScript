import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ExecutionSessionProvider } from './components/SessionContext';
import { ExecussionSessionView } from './components/ExecussionSessionView';
import { OpenFileModal } from './OpenFileModal';
import { APP_TYPE } from './backend';

export function App() {
    return (
        <Router basename={APP_TYPE=='web'?'/fshidden':'/'}>
            <ExecutionSessionProvider>
                <Routes>
                    <Route
                        path="/"
                        element={
                            <ExecussionSessionView
                                initialFile=""
                                initialNodePath=""
                            />
                        }
                    />
                    <Route path="/open-dialog-web" element={<OpenFileModal />} />
                </Routes>
            </ExecutionSessionProvider>
        </Router>
    );
}

export default App;
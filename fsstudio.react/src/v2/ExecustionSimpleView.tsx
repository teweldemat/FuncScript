import React, { useState } from 'react';
import { useExecutionSession } from './ExecutionSessionProvider';

export function ExecustionSimpleView() {
    const context = useExecutionSession();
    const [filePath, setFilePath] = useState('');
    const [nodePath, setNodePath] = useState('');
    const [selectedSession, setSelectedSession] = useState<string>('');
    const [selectedNode, setSelectedNode] = useState<string>('');

    if (!context) return null;
    const { sessions, createSession, loadNode, evaluateNode } = context;
    const sessionKeys = Object.keys(sessions);

    const handleCreateSession = async () => {
        const newSessionId = await createSession(filePath);
        setSelectedSession(newSessionId);
        setFilePath('');
    };

    const handleLoadNode = async () => {
        if (!selectedSession) return;
        await loadNode(selectedSession, nodePath);
        setSelectedNode(nodePath);
        setNodePath('');
    };

    const handleEvaluateNode = async () => {
        if (!selectedSession || !selectedNode) return;
        await evaluateNode(selectedSession, selectedNode);
    };

    return (
        <div>
            <div>
                <input
                    value={filePath}
                    onChange={(e) => setFilePath(e.target.value)}
                    placeholder="File path"
                />
                <button onClick={handleCreateSession}>Load Session</button>
            </div>
            <div>
                <select
                    value={selectedSession}
                    onChange={(e) => setSelectedSession(e.target.value)}
                >
                    <option value="">-- Select Session --</option>
                    {sessionKeys.map((sid) => (
                        <option key={sid} value={sid}>
                            {sid} ({sessions[sid].filePath})
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <input
                    value={nodePath}
                    onChange={(e) => setNodePath(e.target.value)}
                    placeholder="Node path"
                />
                <button onClick={handleLoadNode}>Load Node</button>
            </div>
            <div>
                <select
                    value={selectedNode}
                    onChange={(e) => setSelectedNode(e.target.value)}
                >
                    <option value="">-- Select Node --</option>
                    {selectedSession
                        ? Object.keys(sessions[selectedSession]?.nodes || {}).map((np) => (
                              <option key={np} value={np}>
                                  {np}
                              </option>
                          ))
                        : null}
                </select>
                <button onClick={handleEvaluateNode}>Evaluate Selected Node</button>
            </div>
            <div>
                {sessionKeys.map((sid) => (
                    <div key={sid}>
                        <h4>Session: {sid}</h4>
                        <ul>
                            {Object.entries(sessions[sid].nodes).map(([path, node]) => {
                                let displayName = node.name;
                                if (node.evaluating) {
                                    displayName += ' evaluating..';
                                } else if (node.evaluationRes) {
                                    displayName += ` ${node.evaluationRes}`;
                                }
                                return (
                                    <li key={path}>
                                        {path} - {displayName}
                                    </li>
                                );
                            })}
                        </ul>
                    </div>
                ))}
            </div>
        </div>
    );
}

import React, { useState, useEffect } from 'react';
import { useExecutionSession } from './ExecutionSessionProvider';
import FileTree from './FileTree';

export function FileTreeExecutionView() {
  const { sessions, createSession, loadNode, evaluateNode } = useExecutionSession()!;
  const [selectedFile, setSelectedFile] = useState('');
  const [sessionId, setSessionId] = useState('');
  const [nodePath, setNodePath] = useState('');
  const [selectedNode, setSelectedNode] = useState('');

  useEffect(() => {
    if (selectedFile) {
      const existingSession = Object.values(sessions).find(
        (session) => session.filePath === selectedFile
      );
      if (existingSession) {
        setSessionId(existingSession.sessionId);
      } else {
        createSession(selectedFile).then(setSessionId).catch(console.error);
      }
    }
  }, [selectedFile, sessions, createSession]);

  const handleLoadNode = async () => {
    if (!sessionId) return;
    await loadNode(sessionId, nodePath);
    setSelectedNode(nodePath);
    setNodePath('');
  };

  const handleEvaluateNode = async () => {
    if (!sessionId || !selectedNode) return;
    await evaluateNode(sessionId, selectedNode);
  };

  const currentSession = sessionId ? sessions[sessionId] : null;
  const nodes = currentSession ? currentSession.nodes : {};

  return (
    <div style={{ display: 'flex' }}>
      <FileTree onSelected={setSelectedFile} initiallySelectedPath='' />
      <div style={{ flex: 1, padding: '16px' }}>
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
            {Object.keys(nodes).map((np) => (
              <option key={np} value={np}>
                {np}
              </option>
            ))}
          </select>
          <button onClick={handleEvaluateNode}>Evaluate Selected Node</button>
        </div>
        <div>
          {sessionId && (
            <div key={sessionId}>
              <h4>Session: {sessionId}</h4>
              <ul>
                {Object.entries(nodes).map(([path, node]) => {
                  let displayName = node.name;
                  if (node.evaluating) {
                    displayName += ' evaluating...';
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
          )}
        </div>
      </div>
    </div>
  );
}

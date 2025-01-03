import React, { useState, useEffect } from 'react';
import { useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeTree from './ExpressionNodeTree';

interface ExecussionSessionViewProps {
  selectedFile: string;
}

export function ExecussionSessionView({ selectedFile }: ExecussionSessionViewProps) {
  const { sessions, createSession, evaluateNode } = useExecutionSession()!;
  const [sessionId, setSessionId] = useState('');
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

  const handleEvaluateNode = async () => {
    if (!sessionId || !selectedNode) return;
    await evaluateNode(sessionId, selectedNode);
  };

  const currentSession = sessionId ? sessions[sessionId] : null;
  const nodes = currentSession ? currentSession.nodes : {};

  return (
    <div style={{ flex: 1, padding: '16px' }}>
      {sessionId && (
        <ExpressionNodeTree
          sessionId={sessionId}
          rootNodePath={null}
          onSelect={(nodePath) => setSelectedNode(nodePath ?? '')}
          selectedNode={selectedNode}
        />
      )}
      <button onClick={handleEvaluateNode} disabled={!selectedNode}>
        Evaluate Selected Node
      </button>
      {sessionId && (
        <div key={sessionId}>
          <h4>Session: {sessionId}</h4>
          <ul>
            {Object.entries(nodes).map(([path, node]) => {
              let displayName = node.name;
              if (node.evaluating) {
                displayName += ' (evaluating...)';
              } else if (node.evaluationRes) {
                displayName += ` -> ${node.evaluationRes}`;
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
  );
}

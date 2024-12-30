
// RemoteLogger.tsx
import React, { useEffect, useRef } from 'react';

const RemoteLogger: React.FC<{ messages: string[] }> = ({ messages }) => {
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (bottomRef.current) {
      bottomRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages]);

  return (
    <div style={{ maxHeight: '100%', overflowY: 'auto' }}>
      <ul>
        {messages.map((message, index) => (
          <pre
            style={{
              whiteSpace: 'pre-wrap',
              wordWrap: 'break-word',
              overflowWrap: 'break-word',
              border: '1px solid #ccc',
              padding: '10px',
              fontFamily: '"Lucida Console", monospace',
            }}
            key={index}
          >
            {message}
          </pre>
        ))}
      </ul>
      <div ref={bottomRef} />
    </div>
  );
};

export default RemoteLogger;
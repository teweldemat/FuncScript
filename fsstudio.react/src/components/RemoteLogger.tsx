import React, { useEffect, useRef } from 'react';

const RemoteLogger: React.FC<{ messages: string[] }> = ({ messages }) => {
  const bottomRef = useRef<HTMLDivElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (containerRef.current) {
      containerRef.current.scrollTop = containerRef.current.scrollHeight;
    }
  }, [messages]);

  return (
    <div
      ref={containerRef}
      style={{ maxHeight: '300px', overflowY: 'auto' }}
    >
      <ul>
        {messages.map((message, index) => (
          <pre
            style={{
              whiteSpace: 'pre-wrap',
              overflowWrap: 'anywhere',
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
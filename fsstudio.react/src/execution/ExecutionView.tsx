import React, { useState, useEffect, useRef } from 'react';
import { Grid, Typography, Tab, Tabs, Box, Toolbar, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';

import EvalNodeComponent, { ExpressionType } from './EvalNodeComponent';
import { SERVER_URL, SERVER_WS_URL } from '../backend';
import axios from 'axios';
import TextLogger from './RemoteLogger';
import { EditorView, useCodeMirror } from '@uiw/react-codemirror';
import { javascript } from '@codemirror/lang-javascript';
import ReactMarkdown from 'react-markdown';

interface ErrorItem {
  type: string;
  message: string;
  stackTrace?: string;
}

interface ErrorData {
  errors: ErrorItem[];
}

const CodeEditor: React.FC<{ expression: string | null; setExpression: (exp: string) => void }> = ({
  expression,
  setExpression,
}) => {
  const editorRef = useRef<HTMLDivElement | null>(null);
  useCodeMirror({
    container: editorRef.current,
    value: expression ?? '',
    extensions: [javascript(),EditorView.lineWrapping],
    onChange: (value) => {
      setExpression(value);
    },
  });

  return <div ref={editorRef} style={{ height: '100%', overflow: 'scroll', border: '1px solid #ccc' }} />;
};

const ExecutionView: React.FC<{
  sessionId: string;
  initiallySelectedNode: string | null;
  onNodeSelect: (nodePath: string | null) => void;
}> = ({ sessionId, initiallySelectedNode, onNodeSelect }) => {
  const [selectedNode, setSelectedNode] = useState<string | null>(null);
  const [expression, setExpression] = useState<string | null>(null);
  const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
  const [resultText, setResultText] = useState('');
  const [tabIndex, setTabIndex] = useState(0);
  const [saveStatus, setSaveStatus] = useState('All changes saved');
  const [messages, setMessages] = useState<string[]>([]);
  const [ws, setWs] = useState<WebSocket | null>(null);
  const [activeSessionId, setActiveSessionId] = useState<string | null>(null);
  const [markdown, setMarkdown] = useState<string>('');
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (activeSessionId !== null) {
      if (selectedNode !== null && expression !== lastSavedExpression) {
        saveExpression(selectedNode!, expression, false);
      }
    }
    setSelectedNode(null);
    setExpression(null);
    setActiveSessionId(sessionId);
    setMessages([]);
    setResultText('');
  }, [sessionId]);

  const handleNodeSelect = (nodePath: string | null) => {
    if (selectedNode && expression !== lastSavedExpression) {
      saveExpression(selectedNode!, expression, false);
    }
    if (nodePath == null) {
      setSelectedNode(null);
      setExpression('');
      setLastSavedExpression(null);
      setSaveStatus('All changes saved');
      return;
    }
    axios
      .get(`${SERVER_URL}/api/sessions/${sessionId}/node`, { params: { nodePath } })
      .then((response) => {
        setSelectedNode(nodePath);
        setExpression(response.data.expression ?? '');
        setLastSavedExpression(response.data.expression);
        setSaveStatus('All changes saved');
      })
      .catch((error) => console.error('Failed to fetch node:', error));
    onNodeSelect(nodePath);
  };

  const saveExpression = (nodePath: string, newExpression: string | null, thenEvalute: boolean) => {
    if (newExpression === null) return;
    axios
      .post(`${SERVER_URL}/api/sessions/${activeSessionId}/node/expression/${nodePath}`, {
        expression: newExpression,
      })
      .then(() => {
        setLastSavedExpression(newExpression);
        setSaveStatus('All changes saved');
        if (thenEvalute) executeExpression();
      })
      .catch((error) => {
        console.error('Failed to save expression:', error);
        setSaveStatus('Failed to save changes');
      });
  };

  function formatErrorData(errorData: ErrorData): string {
    const errorsText = errorData.errors
      .map((error, index) => {
        return `Error ${index + 1}:\nType: ${error.type}\nMessage: ${error.message}\nStackTrace: ${
          error.stackTrace || 'N/A'
        }`;
      })
      .join('\n\n');
    return errorsText;
  }

  const executeExpression = () => {
    if (!selectedNode) return;
    if (selectedNode && expression === lastSavedExpression) {
      axios
        .get(`${SERVER_URL}/api/sessions/${activeSessionId}/node/value`, { params: { nodePath: selectedNode } })
        .then((response) => {
          if (typeof response.data === 'string') {
            setResultText(response.data);
          } else {
            setResultText(JSON.stringify(response.data));
          }
          setTabIndex(1);
        })
        .catch((error) => {
          console.error('Failed to evaluate expression:', error);
          if (error.response) {
            setResultText(formatErrorData(error.response.data as ErrorData));
          } else {
            setResultText('Failed to evaluate expression');
          }
          setTabIndex(1);
        });
    }
  };

  useEffect(() => {
    const websocket = new WebSocket(SERVER_WS_URL);
    websocket.onmessage = (event) => {
      const msg = JSON.parse(event.data);
      switch (msg.cmd) {
        case 'log':
          setTabIndex(2);
          setMessages((prev) => [...prev, msg.data]);
          break;
        case 'clear':
          setMessages((prev) => []);
          break;
        case 'markdown':
          setTabIndex(3);
          setMarkdown(msg.data);
          break;
      }
    };
    setWs(websocket);

    return () => {
      websocket.close();
    };
  }, []);

  const renderTabContent = () => {
    switch (tabIndex) {
      case 0:
        return (
          <CodeEditor
            expression={expression}
            setExpression={(e) => {
              setExpression(e);
              setSaveStatus('Unsaved changes');
            }}
          />
        );
      case 1:
        return (
          <pre
            style={{
              whiteSpace: 'pre-wrap',
              wordWrap: 'break-word',
              overflowWrap: 'break-word',
              border: '1px solid #ccc',
              padding: '10px',
              fontFamily: '"Lucida Console", monospace',
            }}
          >
            {resultText}
          </pre>
        );
      case 2:
        return <TextLogger messages={messages} />;
      case 3:
        return <ReactMarkdown children={markdown} />;
      default:
        return null;
    }
  };

  const handleCopy = () => {
    navigator.clipboard.writeText(resultText);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <Grid container spacing={2} style={{ height: '100vh' }}>
      <Grid item xs={8} style={{ display: 'flex', flexDirection: 'column' }}>
        <Toolbar>
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <IconButton onClick={executeExpression} color="primary">
              <PlayArrowIcon />
            </IconButton>
            <IconButton
              onClick={() =>
                selectedNode &&
                expression !== lastSavedExpression &&
                saveExpression(selectedNode!, expression, false)
              }
              color="secondary"
            >
              <SaveIcon />
            </IconButton>
            <IconButton onClick={handleCopy} color="primary">
              {copied ? 'Copied' : <ContentCopyIcon />}
            </IconButton>
          </Box>
          <Box sx={{ marginLeft: 'auto', textAlign: 'right' }}>
            <Typography variant="body2" color="textSecondary">
              {selectedNode ? `${selectedNode} [${saveStatus}]` : `[${saveStatus}]`}
            </Typography>
          </Box>
        </Toolbar>
        <Tabs value={tabIndex} onChange={(event, newValue) => setTabIndex(newValue)} aria-label="Data tabs">
          <Tab label="Script" />
          <Tab label="Result" />
          <Tab label="Log" />
          <Tab label="Document" />
        </Tabs>
        <Box sx={{ flexGrow: 1, borderBottom: 1, borderColor: 'divider', overflow: 'auto' }}>
          {renderTabContent()}
        </Box>
      </Grid>
      <Grid item xs={4} style={{ display: 'flex', flexDirection: 'column' }}>
        {sessionId && (
          <EvalNodeComponent
            node={{
              name: 'Root Node',
              expressionType: ExpressionType.FuncScript,
              childrenCount: 0,
              expression: null,
            }}
            sessionId={sessionId}
            onSelect={handleNodeSelect}
            onModify={() => {}}
            selectedNode={selectedNode}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default ExecutionView;
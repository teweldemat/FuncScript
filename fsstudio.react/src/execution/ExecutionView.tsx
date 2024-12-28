import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Grid, Typography, Tab, Tabs, Box, Toolbar, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import axios from 'axios';

import EvalNodeComponent, { ExpressionType } from './EvalNodeComponent';
import TextLogger from './RemoteLogger';
import ReactMarkdown from 'react-markdown';
import { SERVER_URL, SERVER_WS_URL } from '../backend';
import CodeEditor from '../code-editor/CodeEditor';

interface ErrorItem {
  type: string;
  message: string;
  stackTrace?: string;
}

interface ErrorData {
  errors: ErrorItem[];
}

const ExecutionView: React.FC<{
  sessionId: string;
  initiallySelectedNode: string | null;
  onNodeSelect: (nodePath: string | null) => void;
}> = ({ sessionId, initiallySelectedNode, onNodeSelect }) => {
  const [selectedNode, setSelectedNode] = useState<string | null>(null);
  const [expression, setExpression] = useState<string | null>(null);
  const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
  const [saveStatus, setSaveStatus] = useState('All changes saved');
  const [resultText, setResultText] = useState('');
  const [tabIndex, setTabIndex] = useState(0);
  const [ws, setWs] = useState<WebSocket | null>(null);
  const [messages, setMessages] = useState<string[]>([]);
  const [markdown, setMarkdown] = useState<string>('');
  const [copied, setCopied] = useState(false);
  const [selectedExpressionType, setSelectedExpressionType] = useState<ExpressionType>(
    ExpressionType.FuncScript
  );

  const saveTimerRef = useRef<NodeJS.Timeout | null>(null);
  const [savingInProgress, setSavingInProgress] = useState(false);
  const [queuedExpression, setQueuedExpression] = useState<string | null>(null);

  useEffect(() => {
    setSelectedNode(null);
    setExpression(null);
    setLastSavedExpression(null);
    setSaveStatus('All changes saved');
    setResultText('');
    setMessages([]);
    setMarkdown('');
  }, [sessionId]);

  useEffect(() => {
    if (expression === lastSavedExpression) {
      setSaveStatus('All changes saved');
    } else {
      setSaveStatus('Unsaved changes');
    }
  }, [expression, lastSavedExpression]);

  const executeExpression = useCallback(() => {
    if (!selectedNode) return;
    if (expression === lastSavedExpression) {
      axios
        .get(`${SERVER_URL}/api/sessions/${sessionId}/node/value`, {
          params: { nodePath: selectedNode },
        })
        .then((response) => {
          if (typeof response.data === 'string') {
            setResultText(response.data);
          } else {
            setResultText(JSON.stringify(response.data, null, 2));
          }
          setTabIndex(1);
        })
        .catch((error) => {
          if (error.response && error.response.data) {
            setResultText(formatErrorData(error.response.data as ErrorData));
          } else {
            setResultText('Failed to evaluate expression');
          }
          setTabIndex(1);
        });
    }
  }, [expression, lastSavedExpression, selectedNode, sessionId]);

  const saveExpression = useCallback(
    async (nodePath: string, newExpression: string | null, thenEvaluate: boolean) => {
      if (newExpression === null) return;
      try {
        setSavingInProgress(true);
        await axios.post(`${SERVER_URL}/api/sessions/${sessionId}/node/expression/${nodePath}`, {
          expression: newExpression,
        });
        setLastSavedExpression(newExpression);
        setSaveStatus('All changes saved');
        if (thenEvaluate) {
          executeExpression();
        }
      } catch {
        setSaveStatus('Failed to save changes');
      } finally {
        setSavingInProgress(false);
      }
    },
    [executeExpression, sessionId]
  );

  const handleNodeSelect = (nodePath: string | null) => {
    if (selectedNode && expression !== lastSavedExpression) {
      saveExpression(selectedNode, expression, false);
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
        setSelectedExpressionType(response.data.expressionType);
        setSaveStatus('All changes saved');
      });
    onNodeSelect(nodePath);
  };

  function formatErrorData(errorData: ErrorData): string {
    return errorData.errors
      .map((error, index) => {
        return `Error ${index + 1}:
Type: ${error.type}
Message: ${error.message}
StackTrace: ${error.stackTrace || 'N/A'}
`;
      })
      .join('\n');
  }

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
          setMessages([]);
          break;
        case 'markdown':
          setTabIndex(3);
          setMarkdown(msg.data);
          break;
        default:
          break;
      }
    };
    setWs(websocket);

    return () => {
      websocket.close();
    };
  }, []);

  const handleCopy = () => {
    navigator.clipboard.writeText(resultText);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleClearLog = () => {
    setMessages([]);
  };

  useEffect(() => {
    if (!selectedNode || expression == null) return;
    if (saveTimerRef.current) {
      clearTimeout(saveTimerRef.current);
    }
    if (expression !== lastSavedExpression) {
      setSaveStatus('Unsaved changes');
      setQueuedExpression(expression);

      saveTimerRef.current = setTimeout(() => {
        const doSave = async () => {
          if (savingInProgress) {
            saveTimerRef.current = setTimeout(doSave, 200);
            return;
          }
          if (queuedExpression !== null && queuedExpression !== lastSavedExpression) {
            await saveExpression(selectedNode, queuedExpression, false);
            setQueuedExpression(null);
          }
        };
        doSave();
      }, 1000);
    }

    return () => {
      if (saveTimerRef.current) {
        clearTimeout(saveTimerRef.current);
      }
    };
  }, [
    expression,
    selectedNode,
    lastSavedExpression,
    queuedExpression,
    savingInProgress,
    saveExpression,
  ]);

  const renderTabContent = () => {
    switch (tabIndex) {
      case 0:
        return (
          <CodeEditor
            key={selectedNode || 'no-node'}
            expression={expression}
            setExpression={(value) => setExpression(value)}
            expressionType={selectedExpressionType}
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
        return <ReactMarkdown>{markdown}</ReactMarkdown>;
      default:
        return null;
    }
  };

  const isSaveDisabled = expression === lastSavedExpression;

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
                selectedNode && expression && !isSaveDisabled
                  ? saveExpression(selectedNode, expression, false)
                  : null
              }
              color="secondary"
              disabled={isSaveDisabled}
            >
              <SaveIcon />
            </IconButton>
            <IconButton onClick={handleCopy} color="primary">
              {copied ? 'Copied' : <ContentCopyIcon />}
            </IconButton>
            <IconButton onClick={handleClearLog} color="primary">
              <ClearAllIcon />
            </IconButton>
          </Box>
          <Box sx={{ marginLeft: 'auto', textAlign: 'right' }}>
            <Typography variant="body2" color="textSecondary">
              {selectedNode
                ? `${selectedNode} [${saveStatus}]`
                : `[${saveStatus}]`}
            </Typography>
          </Box>
        </Toolbar>
        <Tabs
          value={tabIndex}
          onChange={(event, newValue) => setTabIndex(newValue)}
          aria-label="Data tabs"
        >
          <Tab label="Script" />
          <Tab label="Result" />
          <Tab label="Log" />
          <Tab label="Document" />
        </Tabs>
        <Box
          sx={{ flexGrow: 1, borderBottom: 1, borderColor: 'divider', overflow: 'auto' }}
        >
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
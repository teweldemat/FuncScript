import React, { useState, useEffect, useRef, useCallback, useMemo } from 'react';
import { Grid, Typography, Tab, Tabs, Box, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import axios from 'axios';
import ReactMarkdown from 'react-markdown';
import { SERVER_URL, SERVER_WS_URL } from '../backend';
import CodeEditor from '../code-editor/CodeEditor'; // Adjust your import
import TextLogger from './RemoteLogger';           // Adjust your import
import { ExpressionType, useFsStudio } from '../FsStudioProvider';
import { EvalNodeTree } from './EvalNodeTree';

interface ExecutionViewProps {
  sessionId: string;
  initiallySelectedNode: string | null;
  onNodeSelect: (nodePath: string | null) => void;
}

const ExecutionView: React.FC<ExecutionViewProps> = ({
  sessionId,
  initiallySelectedNode,
  onNodeSelect,
}) => {
  const { nodeEvaluations } = useFsStudio(); 
  const [selectedNode, setSelectedNode] = useState<string | null>(
    initiallySelectedNode
  );
  const [expression, setExpression] = useState<string | null>(null);
  const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
  const [selectedChildrenCount, setSelectedChildrenCount] = useState<number>(0);

  const [saveStatus, setSaveStatus] = useState('All changes saved');
  const [resultText, setResultText] = useState('');
  const [tabIndex, setTabIndex] = useState(0);
  const [ws, setWs] = useState<WebSocket | null>(null);
  const [messages, setMessages] = useState<string[]>([]);
  const [markdown, setMarkdown] = useState<string>('');
  const [copied, setCopied] = useState(false);
  const [selectedExpressionType, setSelectedExpressionType] =
    useState<ExpressionType>(ExpressionType.FuncScript);

  const saveTimerRef = useRef<NodeJS.Timeout | null>(null);
  const [savingInProgress, setSavingInProgress] = useState(false);
  const [queuedExpression, setQueuedExpression] = useState<string | null>(null);

  useEffect(() => {
    setSelectedNode(initiallySelectedNode);
    setSaveStatus('All changes saved');
    setResultText('');
    setMessages([]);
    setMarkdown('');
  }, [sessionId, initiallySelectedNode]);

  useEffect(() => {
    if (expression === lastSavedExpression) {
      setSaveStatus('All changes saved');
    } else {
      setSaveStatus('Unsaved changes');
    }
  }, [expression, lastSavedExpression]);

  const executeExpression = useCallback(() => {
    if (!selectedNode) return;
    axios
      .post(`${SERVER_URL}/api/sessions/${sessionId}/node/evaluate`, null, {
        params: { nodePath: selectedNode },
      })
      .then(() => {
        // The actual result will come in WebSocket messages
      })
      .catch(() => {
        // handle error
      });
  }, [selectedNode, sessionId]);

  const saveExpression = useCallback(
    async (nodePath: string, newExpression: string | null, thenEvaluate: boolean) => {
// If node is parent, skip
     if (selectedChildrenCount > 0 || selectedExpressionType === ExpressionType.FsStudioParentNode) {
       return;
     }
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

  const handleNodeSelectInternal = (nodePath: string | null) => {
    setSelectedNode(nodePath);
    onNodeSelect(nodePath);
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
          setMessages([]);
          break;
        case 'markdown':
          setTabIndex(3);
          setMarkdown(msg.data);
          break;
        case 'evaluation_success':
          console.log(`eval success:${msg.data.sessionId} ${msg.data.result}`)
          // handle success
          break;
        case 'evaluation_error':
          // handle error
          break;
        default:
          break;
      }
    };
    setWs(websocket);

    return () => {
      websocket.close();
    };
  }, [sessionId]);

  const handleCopy = () => {
    navigator.clipboard.writeText(resultText);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const handleClearLog = () => {
    setMessages([]);
  };

  // Auto-save
  useEffect(() => {
    if (!selectedNode || expression == null) return;
   if (selectedChildrenCount > 0 || selectedExpressionType === ExpressionType.FsStudioParentNode) {
           return;
         }
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
    selectedChildrenCount,
    selectedExpressionType
  ]);

  const isSaveDisabled = expression === lastSavedExpression;

  const memoizedRootNode = useMemo(
    () => ({
      name: 'Root Node',
      path: null,
      expression: null,
      expressionType: ExpressionType.FuncScript,
      childrenCount: 0,
    }),
    [sessionId]
  );
  
  const displayedResult = selectedNode ? nodeEvaluations[selectedNode] || '' : '';

  return (
    <Grid container spacing={2} sx={{ height: '100vh', overflow: 'hidden' }}>
      <Grid item xs={8} container direction="column" wrap="nowrap" sx={{ height: '100%' }}>
        {/* Tabs Header */}
        <Grid
          item
          sx={{ position: 'sticky', top: 0, zIndex: 100, backgroundColor: 'background.paper' }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', borderBottom: 1, borderColor: 'divider' }}>
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
            <Box sx={{ marginLeft: 'auto', display: 'flex', alignItems: 'center' }}>
              {tabIndex === 0 && (
                <>
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
                </>
              )}
              {tabIndex === 1 && (
                <IconButton onClick={handleCopy} color="primary">
                  {copied ? 'Copied' : <ContentCopyIcon />}
                </IconButton>
              )}
              {tabIndex === 2 && (
                <IconButton onClick={handleClearLog} color="primary">
                  <ClearAllIcon />
                </IconButton>
              )}
              <Box sx={{ marginLeft: 2 }}>
                <Typography variant="body2" color="textSecondary">
                  {selectedNode ? `${selectedNode} [${saveStatus}]` : `[${saveStatus}]`}
                </Typography>
              </Box>
            </Box>
          </Box>
        </Grid>
        {/* Tabs Content */}
        <Grid item sx={{ flex: 1, overflow: 'auto' }}>
          {/* Script Tab */}
          <Box
            sx={{
              display: tabIndex === 0 ? 'flex' : 'none',
              flexDirection: 'column',
              height: '100%',
            }}
          >
            <CodeEditor
              key={selectedNode || 'no-node'}
              expression={expression}
              setExpression={(value) => setExpression(value)}
              expressionType={selectedExpressionType}
            />
          </Box>
          {/* Result Tab */}
          <Box
            sx={{
              display: tabIndex === 1 ? 'flex' : 'none',
              flexDirection: 'column',
              height: '100%',
              overflow: 'auto',
            }}
          >
            <pre
              style={{
                whiteSpace: 'pre-wrap',
                wordWrap: 'break-word',
                overflowWrap: 'break-word',
                border: '1px solid #ccc',
                padding: '10px',
                fontFamily: '"Lucida Console", monospace',
                flex: 1,
              }}
            >
               {displayedResult}
            </pre>
          </Box>
          {/* Log Tab */}
          <Box
            sx={{
              display: tabIndex === 2 ? 'flex' : 'none',
              flexDirection: 'column',
              height: '100%',
              overflow: 'auto',
            }}
          >
            <TextLogger messages={messages} />
          </Box>
          {/* Document Tab */}
          <Box
            sx={{
              display: tabIndex === 3 ? 'flex' : 'none',
              flexDirection: 'column',
              height: '100%',
              overflow: 'auto',
            }}
          >
            <ReactMarkdown>{markdown}</ReactMarkdown>
          </Box>
        </Grid>
      </Grid>

      <Grid item xs={4} sx={{ height: '100%', overflow: 'auto' }}>
        {sessionId && (
          <EvalNodeTree
            rootNode={memoizedRootNode}
            sessionId={sessionId}
            onSelect={handleNodeSelectInternal}
            selectedNode={selectedNode}
          />
        )}
      </Grid>
    </Grid>
  );
};

export default ExecutionView;
// ExecussionSessionView.tsx
import React, {
    useState,
    useEffect,
    useRef,
    useCallback,
    useMemo,
} from 'react';
import { Grid, Typography, Tab, Tabs, Box, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import axios from 'axios';
import ReactMarkdown from 'react-markdown';
import { SERVER_URL } from '../backend';
import { findNodeByPath, useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeTree from './ExpressionNodeTree';
import CodeEditor from '../code-editor/CodeEditor';
import TextLogger from '../components/RemoteLogger';
import { ExpressionType } from '../FsStudioProvider';

interface ExecussionSessionViewProps {
    selectedFile: string;
}

export function ExecussionSessionView({ selectedFile }: ExecussionSessionViewProps) {
    const {
        sessions,
        createSession,
        evaluateNode,
        clearSessionLog
    } = useExecutionSession()!;
    const [sessionId, setSessionId] = useState('');
    const [selectedNode, setSelectedNode] = useState<string | null>(null);
    const [expression, setExpression] = useState<string | null>(null);
    const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
    const [saveStatus, setSaveStatus] = useState('All changes saved');
    const [tabIndex, setTabIndex] = useState(0);
    const [copied, setCopied] = useState(false);
    const [queuedExpression, setQueuedExpression] = useState<string | null>(null);
    const [savingInProgress, setSavingInProgress] = useState(false);

    useEffect(() => {
        if (selectedFile) {
            const existingSession = Object.values(sessions).find(
                (s) => s.filePath === selectedFile
            );
            if (existingSession) {
                setSessionId(existingSession.sessionId);
            } else {
                createSession(selectedFile)
                    .then((newSessionId) => setSessionId(newSessionId))
                    .catch(console.error);
            }
        }
    }, [selectedFile, sessions, createSession]);

    useEffect(() => {
        if (expression === lastSavedExpression) {
            setSaveStatus('All changes saved');
        } else {
            setSaveStatus('Unsaved changes');
        }
    }, [expression, lastSavedExpression]);

    const handleNodeSelect = useCallback((nodePath: string | null) => {
        setSelectedNode(nodePath);
    }, []);

    const handleCopy = useCallback(() => {
        if (!selectedNode || !sessionId) return;
        const displayedResult =findNodeByPath(sessions[sessionId]?.nodes,selectedNode)?.evaluationRes || '';
        navigator.clipboard.writeText(displayedResult);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    }, [selectedNode, sessionId, sessions]);

    const handleClearLog = useCallback(() => {
        if (sessionId) {
            clearSessionLog(sessionId);
        }
    }, [sessionId, clearSessionLog]);

    const executeExpression = useCallback(async () => {
        if (!sessionId || !selectedNode) return;
        await evaluateNode(sessionId, selectedNode);
    }, [sessionId, selectedNode, evaluateNode]);

    const saveExpression = useCallback(
        async (nodePath: string, newExpression: string | null, thenEvaluate: boolean) => {
            if (!nodePath || newExpression === null) return;
            try {
                setSavingInProgress(true);
                await axios.post(
                    `${SERVER_URL}/api/sessions/${sessionId}/node/expression/${nodePath}`,
                    { expression: newExpression }
                );
                setLastSavedExpression(newExpression);
                setSaveStatus('All changes saved');
                if (thenEvaluate) {
                    await evaluateNode(sessionId, nodePath);
                }
            } catch {
                setSaveStatus('Failed to save changes');
            } finally {
                setSavingInProgress(false);
            }
        },
        [sessionId, evaluateNode]
    );

    const saveTimerRef = useRef<NodeJS.Timeout | null>(null);

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
                    if (queuedExpression && queuedExpression !== lastSavedExpression) {
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

    const isSaveDisabled = expression === lastSavedExpression;
    const displayedResult =
        selectedNode && sessionId
            ? findNodeByPath(sessions[sessionId]?.nodes,selectedNode)?.evaluationRes || ''
            : '';

    const displayedMessages = sessionId ? sessions[sessionId]?.messages || [] : [];
    const displayedMarkdown = sessionId ? sessions[sessionId]?.markdown || '' : '';

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

    return (
        <Grid container spacing={2} sx={{ height: '100vh', overflow: 'hidden' }}>
            <Grid item xs={8} container direction="column" wrap="nowrap" sx={{ height: '100%' }}>
                <Grid
                    item
                    sx={{
                        position: 'sticky',
                        top: 0,
                        zIndex: 100,
                        backgroundColor: 'background.paper',
                    }}
                >
                    <Box
                        sx={{
                            display: 'flex',
                            alignItems: 'center',
                            borderBottom: 1,
                            borderColor: 'divider',
                        }}
                    >
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
                                    {selectedNode
                                        ? `${selectedNode} [${saveStatus}]`
                                        : `[${saveStatus}]`}
                                </Typography>
                            </Box>
                        </Box>
                    </Box>
                </Grid>
                <Grid item sx={{ flex: 1, overflow: 'auto' }}>
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
                            setExpression={(val) => setExpression(val)}
                            expressionType={ExpressionType.FuncScript}
                        />
                    </Box>
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
                    <Box
                        sx={{
                            display: tabIndex === 2 ? 'flex' : 'none',
                            flexDirection: 'column',
                            height: '100%',
                            overflow: 'auto',
                        }}
                    >
                        <TextLogger messages={displayedMessages} />
                    </Box>
                    <Box
                        sx={{
                            display: tabIndex === 3 ? 'flex' : 'none',
                            flexDirection: 'column',
                            height: '100%',
                            overflow: 'auto',
                        }}
                    >
                        <ReactMarkdown>{displayedMarkdown}</ReactMarkdown>
                    </Box>
                </Grid>
            </Grid>
            <Grid item xs={4} sx={{ height: '100%', overflow: 'auto' }}>
                {sessionId && (
                    <ExpressionNodeTree
                        sessionId={sessionId}
                        onSelect={handleNodeSelect}
                        selectedNode={selectedNode ?? ''}
                    />
                )}
            </Grid>
        </Grid>
    );
}
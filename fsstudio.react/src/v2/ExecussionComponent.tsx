import React, { useState, useEffect, useRef, useCallback } from 'react';
import { Grid, Typography, Tab, Tabs, Box, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import axios from 'axios';
import ReactMarkdown from 'react-markdown';
import { SERVER_URL } from '../backend';
import { useExecutionSession, findNodeByPath } from './ExecutionSessionProvider';
import FileTree from './FileTree';
import ExpressionNodeTree from './ExpressionNodeTree';
import CodeEditor from '../code-editor/CodeEditor';
import TextLogger from '../components/RemoteLogger';
import { ExpressionType } from '../FsStudioProvider';

export function ExecussionComponent() {
    const [sessionId, setSessionId] = useState<string | undefined>();
    const [selectedNodePath, setSelectedNode] = useState<string | null>(null);
    const [expression, setExpression] = useState<string | null>(null);
    const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
    const [saveStatus, setSaveStatus] = useState('All changes saved');
    const [copied, setCopied] = useState(false);
    const [tabIndex, setTabIndex] = useState(0);
    const [queuedExpression, setQueuedExpression] = useState<string | null>(null);
    const [savingInProgress, setSavingInProgress] = useState(false);

    const { createSession, loadNode, sessions, evaluateNode, clearSessionLog } = useExecutionSession()!;

    const handleSelect = async (selectedFile: string) => {
        const newSessionId = await createSession(selectedFile);
        setSessionId(newSessionId);
        setSelectedNode(null); // Reset selected node on session change
    };

    const handleNodeSelect = useCallback(async (nodePath: string | null) => {
        setSelectedNode(nodePath);
        if (nodePath && sessionId) {
            const node = await loadNode(sessionId, nodePath);
            if (node) {
                setExpression(node.expression);
                setLastSavedExpression(node.expression);
            }
        }
    }, [sessionId, loadNode]);

    const handleCopy = useCallback(() => {
        if (!selectedNodePath || !sessionId) return;
        const displayedResult = selectedNode?.evaluationRes || '';
        navigator.clipboard.writeText(displayedResult);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    }, [selectedNodePath, sessionId]);

    const handleClearLog = useCallback(() => {
        if (sessionId) {
            clearSessionLog(sessionId);
        }
    }, [sessionId, clearSessionLog]);

    const executeExpression = useCallback(async () => {
        if (!sessionId || !selectedNodePath) return;
        await evaluateNode(sessionId, selectedNodePath);
    }, [sessionId, selectedNodePath, evaluateNode]);

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
                    await evaluateNode(sessionId!, nodePath);
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
        if (!selectedNodePath || expression == null) return;
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
                        await saveExpression(selectedNodePath, queuedExpression, false);
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
        selectedNodePath,
        lastSavedExpression,
        queuedExpression,
        savingInProgress,
        saveExpression,
    ]);

    const selectedNode = findNodeByPath(
        sessions ? (sessions[sessionId!]?.nodes??[]) : [],
        selectedNodePath ?? ''
    );
    const filePath = sessions ? sessions[sessionId!]?.filePath : null;
    const isSaveDisabled = expression === lastSavedExpression;
    const displayedResult = selectedNode?.evaluationRes || '';
    const displayedMessages = sessions[sessionId!]?.messages || [];
    const displayedMarkdown = sessions[sessionId!]?.markdown || '';

    return (
        <Grid container sx={{ height: '100vh', overflow: 'hidden' }} direction="column">
            <Grid item sx={{ display: 'flex', height: '30%' }}>
                <FileTree onSelected={handleSelect} initiallySelectedPath="" />
            </Grid>
            {sessionId && (
                <Grid container item sx={{ flex: 1, overflow: 'hidden' }} wrap="nowrap">
                    <Grid
                        item
                        xs={3}
                        sx={{
                            borderRight: 1,
                            borderColor: 'divider',
                            height: '100%',
                            overflow: 'auto',
                        }}
                    >
                        <ExpressionNodeTree
                            sessionId={sessionId}
                            onSelect={handleNodeSelect}
                            selectedNode={selectedNodePath ?? ''}
                        />
                    </Grid>

                    <Grid
                        item
                        xs={9}
                        container
                        direction="column"
                        wrap="nowrap"
                        sx={{ height: '100%' }}
                    >
                        <Grid item sx={{ height: '50%', overflow: 'auto' }}>
                            <CodeEditor
                                key={selectedNodePath || 'no-node'}
                                expression={expression}
                                setExpression={setExpression}
                                expressionType={ExpressionType.FuncScript}
                            />
                        </Grid>

                        <Grid
                            item
                            sx={{
                                flex: 1,
                                display: 'flex',
                                flexDirection: 'column',
                                borderTop: 1,
                                borderColor: 'divider',
                                overflow: 'auto',
                            }}
                        >
                            <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                                <Tabs
                                    value={tabIndex}
                                    onChange={(event, newValue) => setTabIndex(newValue)}
                                    aria-label="result-log-markdown"
                                >
                                    <Tab label="Result" />
                                    <Tab label="Log" />
                                    <Tab label="Document" />
                                </Tabs>
                            </Box>
                            <Box sx={{ flex: 1, overflow: 'auto' }}>
                                {tabIndex === 0 && (
                                    <Box sx={{ padding: 2 }}>
                                        <Typography variant="h6">Result</Typography>
                                        <IconButton onClick={handleCopy} color="primary">
                                            {copied ? 'Copied' : <ContentCopyIcon />}
                                        </IconButton>
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
                                            {displayedResult}
                                        </pre>
                                    </Box>
                                )}
                                {tabIndex === 1 && (
                                    <Box>
                                        <IconButton onClick={handleClearLog} color="primary">
                                            <ClearAllIcon />
                                        </IconButton>
                                        <TextLogger messages={displayedMessages} />
                                    </Box>
                                )}
                                {tabIndex === 2 && (
                                    <Box sx={{ padding: 2 }}>
                                        <ReactMarkdown>{displayedMarkdown}</ReactMarkdown>
                                    </Box>
                                )}
                            </Box>
                        </Grid>
                    </Grid>
                </Grid>
            )}
        </Grid>
    );
}
import React, {
    useState,
    useEffect,
    useRef,
    useCallback,
} from 'react';
import { Grid, Typography, Box, IconButton } from '@mui/material';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SaveIcon from '@mui/icons-material/Save';
import axios from 'axios';
import { SERVER_URL } from '../backend';
import { findNodeByPath, useExecutionSession } from './ExecutionSessionProvider';
import ExpressionNodeTree from './ExpressionNodeTree';
import { ExpressionType } from '../FsStudioProvider';
import FileTree from './FileTree';
import ExecussionContent from './ExecussionContent';

export function ExecussionSessionView() {
    const {
        createSession,
        loadNode,
        sessions,
        evaluateNode,
        clearSessionLog
    } = useExecutionSession()!;
    const [sessionId, setSessionId] = useState<string | null>(null);
    const [selectedNodePath, setSelectedNode] = useState<string | null>(null);
    const [expression, setExpression] = useState<string | null>(null);
    const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
    const [saveStatus, setSaveStatus] = useState('All changes saved');
    const [copied, setCopied] = useState(false);
    const [queuedExpression, setQueuedExpression] = useState<string | null>(null);
    const [savingInProgress, setSavingInProgress] = useState(false);

    useEffect(() => {
        if (expression === lastSavedExpression) {
            setSaveStatus('All changes saved');
        } else {
            setSaveStatus('Unsaved changes');
        }
    }, [expression, lastSavedExpression]);

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

    const selectedNode = (sessionId && sessions && selectedNodePath)
        ? findNodeByPath(sessions[sessionId]?.nodes ?? [], selectedNodePath)
        : null;

    const handleCopy = useCallback(() => {
        if (!selectedNodePath || !sessionId) return;
        const displayedResult = selectedNode?.evaluationRes || '';
        navigator.clipboard.writeText(displayedResult);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    }, [selectedNodePath, sessionId, selectedNode]);

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

    const filePath = (sessions && sessionId)
        ? sessions[sessionId]?.filePath
        : null;

    const isSaveDisabled = expression === lastSavedExpression;
    const displayedResult = selectedNodePath && sessionId
        ? selectedNode?.evaluationRes || ''
        : '';
    const displayedMessages = sessionId ? sessions[sessionId]?.messages || [] : [];
    const displayedMarkdown = sessionId ? sessions[sessionId]?.markdown || '' : '';

    const handleFileSelect = useCallback(async (selectedFile: string) => {
        if(selectedFile=='')
            setSessionId(null)
        else
        {
            const newSessionId = await createSession(selectedFile);
            setSessionId(newSessionId);
        }
    }, [createSession]);

    return (
        <Grid container sx={{ height: '100vh', overflow: 'auto' }} wrap="nowrap">
            <Grid
                item
                sx={{ width: '30%', borderRight: 1, borderColor: 'divider', display: 'flex', flexDirection: 'column' }}
            >
                <Box sx={{ flex: 1, overflow: 'auto', borderBottom: 1, borderColor: 'divider' }}>
                    {sessionId ? (
                        <ExpressionNodeTree
                            sessionId={sessionId}
                            onSelect={handleNodeSelect}
                            selectedNode={selectedNodePath ?? ''}
                        />
                    ) : (
                        <div>Session not selected</div>
                    )}
                </Box>
                <Box sx={{ height: '50%', overflow: 'auto' }}>
                    <FileTree onSelected={handleFileSelect} initiallySelectedPath="" />
                </Box>
            </Grid>


            <Grid
                item
                sx={{ flex: 1, display: 'flex', flexDirection: 'column' }}
            >
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Box
                        sx={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                            padding: 1,
                        }}
                    >
                        <Typography variant="body1" noWrap>
                            {filePath ? `${filePath}` : '[No file path]'}
                            {selectedNodePath ? ` : ${selectedNodePath}` : ''}
                        </Typography>
                        <Box>
                            <IconButton onClick={executeExpression} color="primary">
                                <PlayArrowIcon />
                            </IconButton>
                            <IconButton
                                onClick={() =>
                                    selectedNodePath && expression && !isSaveDisabled
                                        ? saveExpression(selectedNodePath, expression, false)
                                        : null
                                }
                                color="secondary"
                                disabled={isSaveDisabled}
                            >
                                <SaveIcon />
                            </IconButton>
                            <Typography variant="caption" sx={{ marginLeft: 2 }}>
                                [{saveStatus}]
                            </Typography>
                        </Box>
                    </Box>
                </Box>

                <Box sx={{ flex: 1, overflow: 'auto' }}>
                    <ExecussionContent
                        expression={expression}
                        setExpression={(val) => setExpression(val)}
                        displayedResult={displayedResult}
                        handleCopy={handleCopy}
                        copied={copied}
                        displayedMessages={displayedMessages}
                        handleClearLog={handleClearLog}
                        displayedMarkdown={displayedMarkdown}
                    />
                </Box>
            </Grid>
        </Grid>
    );
}

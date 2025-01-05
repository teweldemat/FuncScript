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
import { findNodeByPath, SessionState, useExecutionSession } from './ExecutionSessionProvider';
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
        clearSessionLog,
        setSelectedNodePath
    } = useExecutionSession()!;
    const [session, setSession] = useState<SessionState | null>(null);
    const [expression, setExpression] = useState<string | null>(null);
    const [lastSavedExpression, setLastSavedExpression] = useState<string | null>(null);
    const [saveStatus, setSaveStatus] = useState('All changes saved');
    const [copied, setCopied] = useState(false);
    const [queuedExpression, setQueuedExpression] = useState<string | null>(null);
    const [savingInProgress, setSavingInProgress] = useState(false);

    useEffect(() => {
        if (!session) return;
        setSession(sessions[session?.sessionId]);
    }, [sessions]);
    useEffect(() => {
        loadNodeData(session?.selectedNodePath??null)
    }, [session]);

    useEffect(() => {
        if (expression === lastSavedExpression) {
            setSaveStatus('All changes saved');
        } else {
            setSaveStatus('Unsaved changes');
        }
    }, [expression, lastSavedExpression]);

    function clearCurrentNode() {
        setExpression('');
        setLastSavedExpression('');

    }
    function loadNodeData(nodePath: string | null) {
        if (!session || !nodePath) {
            clearCurrentNode()
            return;
        }
        const node = findNodeByPath(session, nodePath)
        if (!node) {
            clearCurrentNode();
            return;
        }
        if (!node?.dataLoaded)
        {
            loadNode(session, nodePath).then(newNode => {
                setExpression(newNode?.expression ?? '');
                setLastSavedExpression(newNode?.expression ?? '');
            });
        }
        else
        {
            setExpression(node.expression ?? '');
            setLastSavedExpression(node?.expression ?? '');

        }
    }
    const handleNodeSelect = useCallback(async (nodePath: string | null) => {
        if (session)
            setSelectedNodePath(session, nodePath);
        await loadNodeData(nodePath)
    }, [session, loadNode]);

    const selectedNode = (session?.selectedNodePath)
        ? findNodeByPath(session ?? [], session.selectedNodePath)
        : null;

    const handleCopy = useCallback(() => {
        if (!session?.selectedNodePath) return;
        const displayedResult = selectedNode?.evaluationRes || '';
        navigator.clipboard.writeText(displayedResult);
        setCopied(true);
        setTimeout(() => setCopied(false), 2000);
    }, [session, selectedNode]);

    const handleClearLog = useCallback(() => {
        if (session) {
            clearSessionLog(session);
        }
    }, [session, clearSessionLog]);

    const executeExpression = useCallback(async () => {
        if (!session || !(session?.selectedNodePath)) return;
        await evaluateNode(session, session?.selectedNodePath);
    }, [session, evaluateNode]);

    const saveExpression = useCallback(
        async (nodePath: string, newExpression: string | null, thenEvaluate: boolean) => {
            if (!nodePath || newExpression === null) return;
            try {
                setSavingInProgress(true);
                await axios.post(
                    `${SERVER_URL}/api/sessions/${session?.sessionId}/node/expression/${nodePath}`,
                    { expression: newExpression }
                );
                setLastSavedExpression(newExpression);
                setSaveStatus('All changes saved');
                if (thenEvaluate) {
                    await evaluateNode(session!, nodePath);
                }
            } catch {
                setSaveStatus('Failed to save changes');
            } finally {
                setSavingInProgress(false);
            }
        },
        [session, evaluateNode]
    );

    const saveTimerRef = useRef<NodeJS.Timeout | null>(null);

    useEffect(() => {
        if (!session || !session?.selectedNodePath || expression == null) return;
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
                        await saveExpression(session.selectedNodePath!, queuedExpression, false);
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
        lastSavedExpression,
        queuedExpression,
        savingInProgress,
        saveExpression,
    ]);

    const filePath = session?.filePath ?? null;

    const isSaveDisabled = expression === lastSavedExpression;
    const displayedResult = session?.selectedNodePath && session
        ? selectedNode?.evaluationRes || ''
        : '';
    const displayedMessages = session?.messages || [];
    const displayedMarkdown = session?.markdown || '';

    const handleFileSelect = useCallback(async (selectedFile: string) => {
        if (selectedFile == '') {
            setSession(null);
        } else {
            const newSession = await createSession(selectedFile);
            setSession(newSession);
        }
    }, [createSession]);

    return (
        <Grid container sx={{ height: '100vh', overflow: 'auto' }} wrap="nowrap">
            <Grid
                item
                sx={{ width: '30%', borderRight: 1, borderColor: 'divider', display: 'flex', flexDirection: 'column' }}
            >
                <Box sx={{ flex: 1, overflow: 'auto', borderBottom: 1, borderColor: 'divider' }}>
                    {session ? (
                        <ExpressionNodeTree
                            session={session}
                            onSelect={handleNodeSelect}
                            selectedNode={session?.selectedNodePath ?? ''}
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
                            {session?.selectedNodePath ? ` : ${session?.selectedNodePath}` : ''}
                        </Typography>
                        <Box>
                            <IconButton onClick={executeExpression} color="primary">
                                <PlayArrowIcon />
                            </IconButton>
                            <IconButton
                                onClick={() =>
                                    session?.selectedNodePath && expression && !isSaveDisabled
                                        ? saveExpression(session.selectedNodePath, expression, false)
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
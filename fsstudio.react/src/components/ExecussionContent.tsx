import React, { useState } from 'react';
import { Grid, Tab, Tabs, Box, IconButton } from '@mui/material';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import ReactMarkdown from 'react-markdown';
import CodeEditor from '../code-editor/CodeEditor';
import TextLogger from './RemoteLogger';
import { ExpressionType } from './SessionContext';

interface ExecussionContentProps {
    expression: string | null;
    setExpression: (val: string | null) => void;
    displayedResult: string | null;
    displayedError: string | null;
    handleCopy: () => void;
    copied: boolean;
    displayedMessages: any[];
    handleClearLog: () => void;
    displayedMarkdown: string;
    readOnly: boolean;
}

export default function ExecussionContent({
    expression,
    setExpression,
    displayedResult,
    displayedError,
    handleCopy,
    copied,
    displayedMessages,
    handleClearLog,
    displayedMarkdown,
    readOnly
}: ExecussionContentProps) {
    const [tabIndex, setTabIndex] = useState(0);

    return (
        <Grid container direction="column" wrap="nowrap" sx={{ height: '100%' }}>
            <Grid item sx={{ height: '50%', overflow: 'auto' }}>
                <CodeEditor
                    expression={expression}
                    setExpression={setExpression}
                    expressionType={ExpressionType.FuncScript}
                    readOnly={readOnly}
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
                <Box
                    sx={{
                        borderBottom: 1,
                        borderColor: 'divider',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'space-between',
                    }}
                >
                    <Tabs
                        value={tabIndex}
                        onChange={(event, newValue) => setTabIndex(newValue)}
                        aria-label="result-log-markdown"
                    >
                        <Tab label="Result" />
                        <Tab label="Log" />
                        <Tab label="Document" />
                    </Tabs>
                    {tabIndex === 0 && (
                        <IconButton onClick={handleCopy} color="primary">
                            {copied ? 'Copied' : <ContentCopyIcon />}
                        </IconButton>
                    )}
                    {tabIndex === 1 && (
                        <IconButton onClick={handleClearLog} color="primary">
                            <ClearAllIcon />
                        </IconButton>
                    )}
                </Box>
                <Box sx={{ flex: 1, overflow: 'auto' }}>
                    <Box
                        sx={{
                            display: tabIndex === 0 ? 'block' : 'none',
                            height: '100%'
                        }}
                    >
                        <Box sx={{ height: '100%', padding: 2 }}>
                            <pre
                                style={{
                                    whiteSpace: 'pre-wrap',
                                    overflowWrap: 'anywhere',
                                    border: '1px solid #ccc',
                                    padding: '10px',
                                    fontFamily: '"Lucida Console", monospace',
                                    flex: 1,
                                    color: displayedError ? 'red' : 'inherit'
                                }}
                            >
                                {displayedError || displayedResult}
                            </pre>
                        </Box>
                    </Box>
                    <Box
                        sx={{
                            display: tabIndex === 1 ? 'block' : 'none',
                            height: '100%'
                        }}
                    >
                        <TextLogger messages={displayedMessages} />
                    </Box>
                    <Box
                        sx={{
                            display: tabIndex === 2 ? 'block' : 'none',
                            height: '100%'
                        }}
                    >
                        <Box sx={{ padding: 2 }}>
                            <ReactMarkdown>{displayedMarkdown}</ReactMarkdown>
                        </Box>
                    </Box>
                </Box>
            </Grid>
        </Grid>
    );
}
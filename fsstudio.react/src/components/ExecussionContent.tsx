import React, { useState, useRef } from 'react';
import { Grid, Tab, Tabs, Box, IconButton } from '@mui/material';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';
import ClearAllIcon from '@mui/icons-material/ClearAll';
import ReactMarkdown from 'react-markdown';
import CodeEditor from '../code-editor/CodeEditor';
import TextLogger from './RemoteLogger';
import { ExpressionType, useExecutionSession } from './SessionContext';

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
    sessionId?: string;
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
    readOnly,
    sessionId
}: ExecussionContentProps) {
    const { sessions, sendInput } = useExecutionSession()!;
    const [tabIndex, setTabIndex] = useState(0);
    const [inputText, setInputText] = useState('');
    const inputRef = useRef<HTMLInputElement>(null);

    const handleInputKeyPress = async (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter' && sessionId) {
            const session = sessions[sessionId];
            if (session) {
                try {
                    await sendInput(session, inputText);
                    // Clear input after sending
                    setInputText('');
                    // Select the text after sending
                    if (inputRef.current) {
                        inputRef.current.select();
                    }
                } catch (error) {
                    console.error('Failed to send input:', error);
                }
            }
        }
    };

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
                        gap: 2,
                        px: 2,
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
                    <Box sx={{ flex: 1 }}>
                        <input
                            ref={inputRef}
                            type="text"
                            value={inputText}
                            onChange={(e) => setInputText(e.target.value)}
                            onKeyPress={handleInputKeyPress}
                            style={{
                                width: '100%',
                                padding: '8px',
                                border: '1px solid #ccc',
                                borderRadius: '4px',
                                fontSize: '14px',
                            }}
                            placeholder="Type input and press Enter..."
                        />
                    </Box>
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
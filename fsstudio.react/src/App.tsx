import React from 'react';
import { CssBaseline, Box } from '@mui/material';
import { FsStudioProvider } from './FsStudioProvider';

const App: React.FC = () => {
  return (
    <React.Fragment>
      <CssBaseline />
      <Box sx={{ display: 'flex', height: '100vh' }}>
        <FsStudioProvider />
      </Box>
    </React.Fragment>
  );
};

export default App;
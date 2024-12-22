const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');
const fs = require('fs');
const http = require('http');

let mainWindow, serverProcess;

const gotTheLock = app.requestSingleInstanceLock();
if (!gotTheLock) {
  app.quit();
} else {
  app.on('second-instance', () => {
    if (mainWindow) {
      if (mainWindow.isMinimized()) mainWindow.restore();
      mainWindow.focus();
    }
  });

  function waitForServerAndLoadUrl(url, window) {
    http
      .get(url, res => {
        if (res.statusCode === 200) {
          window.loadURL(url);
        } else {
          setTimeout(() => waitForServerAndLoadUrl(url, window), 1000);
        }
      })
      .on('error', () => {
        setTimeout(() => waitForServerAndLoadUrl(url, window), 1000);
      });
  }

  function createWindow() {
    mainWindow = new BrowserWindow({ width: 800, height: 600 });
    mainWindow.loadURL(`data:text/html,<h1>Starting app..</h1>`);
    mainWindow.webContents.on('did-fail-load', (event, errorCode, errorDescription, validatedURL) => {
      console.error(`Failed to load ${validatedURL}: ${errorDescription} (code: ${errorCode})`);
    });
    mainWindow.on('closed', () => {
      if (serverProcess) {
        serverProcess.kill();
      }
      mainWindow = null;
    });
  }

  app.whenReady().then(() => {
    const serverExec = path.join(
      process.resourcesPath,
      'server',
      'fsstudio.server.fileSystem'
    );

    console.log(`Attempting to launch server from: ${serverExec}`);
    fs.access(serverExec, fs.constants.F_OK, (err) => {
      if (err) {
        console.error('Server file does not exist or is inaccessible:', err);
        createWindow();
      } else {
        console.log('Server file found, launching...');
        serverProcess = spawn(serverExec, ['--urls', 'http://localhost:5091'], { shell: true });
        serverProcess.on('error', (spawnErr) => {
          console.error('Failed to start server:', spawnErr);
        });
        serverProcess.stdout.on('data', (data) => {
          console.log(`Server Output: ${data}`);
        });
        serverProcess.stderr.on('data', (data) => {
          console.error(`Server Error: ${data}`);
        });
        createWindow();
        waitForServerAndLoadUrl('http://localhost:5091', mainWindow);
      }
    });
  });

  app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit();
  });

  app.on('activate', () => {
    if (mainWindow === null) createWindow();
  });
}
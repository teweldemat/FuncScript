const { app, BrowserWindow, dialog } = require('electron');
const { spawn } = require('child_process');
const path = require('path');
const fs = require('fs');
const http = require('http');

let mainWindow, serverProcess;

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

async function createWindow() {
  mainWindow = new BrowserWindow({ width: 800, height: 600 });
  mainWindow.loadURL(`data:text/html,<h1>Starting app..</h1>`);

  // Only open dev tools if in development
  const { default: isDev } = await import('electron-is-dev');
  if (isDev) {
    mainWindow.webContents.openDevTools();
  }

  mainWindow.on('closed', () => {
    if (serverProcess) {
      serverProcess.kill();
    }
    mainWindow = null;
  });
  mainWindow.webContents.on('will-navigate', (event, url) => {
    if (url.endsWith('/open-folder-dialog')) {
      event.preventDefault();
      dialog.showOpenDialog(mainWindow, {
        properties: ['openDirectory'],
      }).then(result => {
        if (!result.canceled && result.filePaths.length > 0) {
          fetch('http://localhost:5099/api/FileSystem/SetRootFolder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ newRootFolder: result.filePaths[0] })
          })
          .then(response => {
            if (response.ok) {
              mainWindow.reload();
            }
          })
          .catch(err => console.error(err));
        }
      });
    }
  });
  
}

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

  app.whenReady().then(async () => {
    const { default: isDev } = await import('electron-is-dev');

    if (isDev) {
      await createWindow();
      waitForServerAndLoadUrl('http://localhost:3000', mainWindow);
    } else {
      const serverExec = path.join(
        process.resourcesPath,
        'server',
        'fsstudio.server.fileSystem'
      );

      fs.access(serverExec, fs.constants.F_OK, async (err) => {
        if (err) {
          await createWindow();
        } else {
          serverProcess = spawn(serverExec, ['--urls', 'http://localhost:5099'], { shell: true });
          serverProcess.stdout.on('data', (data) => {
            console.log(`Server Output: ${data}`);
          });
          serverProcess.stderr.on('data', (data) => {
            console.error(`Server Error: ${data}`);
          });

          await createWindow();
          waitForServerAndLoadUrl('http://localhost:5099', mainWindow);
        }
      });
    }
  });

  app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') app.quit();
  });

  app.on('activate', async () => {
    if (mainWindow === null) await createWindow();
  });
}
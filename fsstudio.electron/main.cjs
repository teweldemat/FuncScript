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

async function createWindow(isDev) {
  mainWindow = new BrowserWindow({ width: 800, height: 600 });
  mainWindow.loadURL(`data:text/html,<h1>Starting ${isDev?'development app':'app'}.</h1>`);
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
          const port = isDev ? 5099 : 5091;
          fetch(`http://localhost:${port}/api/FileSystem/SetRootFolder`, {
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
    } else if (url.endsWith('/create-folder-dialog')) {
      event.preventDefault();
      dialog.showSaveDialog(mainWindow, {
        title: 'Create Folder',
        buttonLabel: 'Create'
      }).then(result => {
        if (!result.canceled && result.filePath) {
          fs.mkdir(result.filePath, { recursive: true }, err => {
            if (!err) {
              const port = isDev ? 5099 : 5091;
              fetch(`http://localhost:${port}/api/FileSystem/SetRootFolder`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ newRootFolder: result.filePath })
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
  });
}

app.whenReady().then(async () => {
  const isDev = !app.isPackaged;
  if (isDev) {
    await createWindow(isDev);
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
        serverProcess = spawn(serverExec, ['--urls', 'http://localhost:5091'], { shell: true });
        serverProcess.stdout.on('data', (data) => {
          console.log(`Server Output: ${data}`);
        });
        serverProcess.stderr.on('data', (data) => {
          console.error(`Server Error: ${data}`);
        });
        await createWindow();
        waitForServerAndLoadUrl('http://localhost:5091', mainWindow);
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
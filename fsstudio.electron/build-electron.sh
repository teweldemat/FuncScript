#!/bin/bash
# Navigate to the React project directory
cd ../fsstudio.react

# Copy the fs.png file to the Electron project's directory
cp public/fs.png ../fsstudio.electron/

# Install dependencies and build the React project
npm install
npm run build

# Copy the build output to the .NET project's wwwroot directory
cp -R build/ ../fsstudio.server.fileSystem/wwwroot/

# Navigate to the .NET project directory and build it
cd ../fsstudio.server.fileSystem
dotnet publish -c Release --self-contained true

# Return to the Electron project directory and create the distribution
cd ../fsstudio.electron
npm run dist
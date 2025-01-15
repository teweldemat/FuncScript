#!/usr/bin/env bash
set -e

cd ../fsstudio.react
npm install
npm run build

cp -R build/ ../FsStudio.Server.FileSystem/wwwroot/

cd ../FsStudio.Server.FileSystem
dotnet publish -c Release

# Use rsync to copy only changed files
rsync -av --delete bin/Release/net8.0/publish/ tewelde@nrlais.gov.et:~/fss/bin/

ssh tewelde@nrlais.gov.et "sudo systemctl restart fsweb"
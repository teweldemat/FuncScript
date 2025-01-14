#!/usr/bin/env bash
set -e

cd ../fsstudio.react
npm install
npm run build

cp -R build/ ../fsstudio.server.fileSystem/wwwroot/

cd ../fsstudio.server.fileSystem
dotnet publish -c Release

# Use rsync to copy only changed files
rsync -av --delete bin/Release/net8.0/publish/ tewelde@nrlais.gov.et:~/fss/bin/

ssh tewelde@nrlais.gov.et "sudo systemctl restart fsweb"
#!/usr/bin/env bash
set -e

cd ../fsstudio.react
npm install
npm run build

cp -R build/ ../FsStudio.Server.FileSystem/wwwroot/

cd ../FsStudio.Server.FileSystem
dotnet publish -c Release

# Use rsync to copy to staging directory first
rsync -av --delete bin/Release/net8.0/publish/ tewelde@nrlais.gov.et:~/stage/

# Use sudo to copy from staging to final destination
ssh tewelde@nrlais.gov.et "sudo cp -R ~/stage/* /usr/bin/fss/bin/"
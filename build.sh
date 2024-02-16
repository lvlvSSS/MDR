#! /bin/bash
echo "start to build MDR ..."

echo "build solution ..."
dotnet build MDR.sln -c Release
echo "build solution end!"

echo "copy the NLog.config to release folder ..."
cp ./MDR.Infrastructure/MDR.Infrastructure.Log/NLog.config ./MDR.Server/bin/Release/net8.0
echo "copy the NLog.config end!"

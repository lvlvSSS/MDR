#! /bin/bash
echo "start to build MDR ..."

echo "build solution ..."
dotnet build MDR.sln -c Release
echo "build solution end!"

# deal with MDR.Infrastructure.Log
echo "ready to deal with MDR.Infrastructure.Log ..."
echo "copy the NLog.config to release folder ..."
cp ./MDR.Infrastructure/MDR.Infrastructure.Log/NLog.config ./MDR.Server/bin/Release/net8.0
echo "copy the NLog.config end!"
echo "MDR.Infrastructure.Log is done!"

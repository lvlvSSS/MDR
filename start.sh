#! /bin/bash
# this script is to start the MDR server.
echo "ready to start MDR server ..."

PUBLISH_SERVER_DIR=$(pwd)/MDR.Server/bin/Release/net8.0

# here to set ASPNETCORE_ENVIRONMENT to decide the runtime environment.
export ASPNETCORE_ENVIRONMENT='Development'
export ASPNETCORE_NELSON="nelson"

dotnet $PUBLISH_SERVER_DIR/MDR.Server.dll

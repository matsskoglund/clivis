#!/bin/bash
set -e
dotnet restore 
dotnet build src/Clivis/project.json
rm -rf $(pwd)bin/Release/netcoreapp1.1/publish
dotnet publish src/Clivis/project.json -c Release -o $(pwd)bin/Release/netcoreapp1.1/publish

#!/bin/bash
set -e
dotnet restore 
dotnet build src/Clivis/project.json
rm -rf $(pwd)bin/Debug/netcoreapp1.1/publish
dotnet publish src/Clivis/project.json -c Debug -o $(pwd)bin/Debug/netcoreapp1.1/publish

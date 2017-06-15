#!/bin/bash
ls -l gitrepo/src
dotnet restore 
dotnet build src/Clivis/Clivis.csproj
#!/bin/bash
ls -l gitrepo/src/Clivis
dotnet restore gitrepo/src/Clivis/Clivis.csproj
dotnet build gitrepo/src/Clivis/Clivis.csproj
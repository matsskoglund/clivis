#!/bin/bash
ls -l gitrepo/src/Clivis
dotnet restore 
dotnet build gitrepo/src/Clivis/Clivis.csproj
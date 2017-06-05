# OLD - FROM microsoft/dotnet:1.0.1-core
FROM microsoft/dotnet:1.1.2-sdk

WORKDIR /app
ENV ASPNETCORE_URLS http://*:5050
EXPOSE 5050
ENTRYPOINT ["dotnet", "Clivis.dll"]
# OLD - COPY . /app
COPY bin/Debug/netcoreapp1.1/publish /app

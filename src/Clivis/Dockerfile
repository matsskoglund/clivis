FROM microsoft/1.0.1-sdk-projectjson
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5050
EXPOSE 5050
ENTRYPOINT ["dotnet", "Clivis.dll"]
COPY . /app

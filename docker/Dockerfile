FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ADD dist /net-core-app
ENV ASPNETCORE_PATHBASE=/net-core-app
VOLUME [ "/net-core-app/config", "/net-core-app/log" ]
WORKDIR /net-core-app
ENTRYPOINT ["dotnet", "NetCoreApp.dll"]

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0
ADD dist /net-core-app
ENV ASPNETCORE_PATHBASE=/net-core-app
ENV ASPNETCORE_URLS=http://+:5000
VOLUME [ "/net-core-app/config", "/net-core-app/log" ]
WORKDIR /net-core-app
ENTRYPOINT ["dotnet", "NetCoreApp.dll"]

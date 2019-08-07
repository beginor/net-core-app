FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
ADD dist /api
ENV ASPNETCORE_PATHBASE=/net-core-app
VOLUME [ "/api/logs" ]
WORKDIR /api
ENTRYPOINT ["dotnet", "Beginor.NetCoreApp.Api.dll"]

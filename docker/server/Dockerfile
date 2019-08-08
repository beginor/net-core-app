FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
ADD dist /net-core-app/api
ENV ASPNETCORE_PATHBASE=/net-core-app
VOLUME [ "/net-core-app/api/log" ]
WORKDIR /net-core-app/api
ENTRYPOINT ["dotnet", "Beginor.NetCoreApp.Api.dll"]

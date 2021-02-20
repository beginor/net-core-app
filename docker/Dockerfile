FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ADD dist /gishub
ENV ASPNETCORE_PATHBASE=/gishub
ENV ASPNETCORE_URLS=http://+:5000
VOLUME [ "/gishub/config", "/gishub/log" ]
WORKDIR /gishub
ENTRYPOINT ["dotnet", "GisHub.dll"]

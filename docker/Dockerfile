FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ADD dist /gishub
ENV ASPNETCORE_PATHBASE=/gishub
VOLUME [ "/gishub/config", "/gishub/log" ]
WORKDIR /gishub
ENTRYPOINT ["dotnet", "GisHub.dll"]

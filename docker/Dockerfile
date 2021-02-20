FROM mcr.microsoft.com/dotnet/runtime-deps:5.0
ENV ASPNETCORE_PATHBASE=/gishub \
    ASPNETCORE_URLS=http://+:5000
ADD dist /gishub
VOLUME [ "/gishub/config", "/gishub/log" ]
WORKDIR /gishub
ENTRYPOINT ["./GisHub"]

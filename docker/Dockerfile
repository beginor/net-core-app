FROM mcr.microsoft.com/dotnet/runtime-deps:7.0
ENV ASPNETCORE_PATHBASE=/gishub \
    ASPNETCORE_URLS=http://+:5000 \
    TZ=Asia/Shanghai
ADD dist /gishub
VOLUME [ "/gishub/config", "/gishub/log", "/gishub/wwwroot", "/gishub/app_cache" ]
WORKDIR /gishub
ENTRYPOINT ["./GisHub"]

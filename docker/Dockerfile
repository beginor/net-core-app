FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_URLS=http://+:8080 \
    TZ=Asia/Shanghai
ADD dist /app
VOLUME [ "/app/config", "/app/log", "/app/wwwroot", "/app/cache" ]
WORKDIR /app
ENTRYPOINT ["./NetCoreApp"]

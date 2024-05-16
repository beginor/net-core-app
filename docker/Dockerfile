FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_HTTP_PORTS=8080 \
    TZ=Asia/Shanghai \
    LANG=C.utf8
ADD dist /app
VOLUME [ "/app/config", "/app/log", "/app/wwwroot", "/app/cache" ]
WORKDIR /app
ENTRYPOINT ["./NetCoreApp"]

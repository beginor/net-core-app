FROM mcr.microsoft.com/dotnet/runtime-deps:9.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_HTTP_PORTS=8080 \
    TZ=Asia/Shanghai \
    LANG=C.utf8
RUN apt-get update && \
    apt-get install -y fonts-freefont-otf fontconfig && \
    fc-cache -f -v && \
    apt-get autoremove -y && \
    rm -rf /var/lib/apt/lists/*
ADD dist /app
VOLUME [ "/app/config", "/app/log", "/app/wwwroot", "/app/cache" ]
WORKDIR /app
ENTRYPOINT ["./NetCoreApp"]

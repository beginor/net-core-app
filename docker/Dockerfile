FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_URLS=http://+:5050 \
    TZ=Asia/Shanghai \
    LANG=C.UTF-8 \
    LC_ALL=C.UTF-8
RUN apt-get update && \
     apt-get install -y --no-install-recommends \
    libgssapi-krb5-2 libkrb5-3 libkrb5support0 \
    fonts-freefont-otf fontconfig && \
    fc-cache -f -v && \
    apt-get autoremove -y && \
    rm -rf /var/lib/apt/lists/*
ADD dist /app
VOLUME ["/app/config", "/app/log", "/app/cache", "/app/storage"]
WORKDIR /app
ENTRYPOINT ["/app/NetCoreApp"]

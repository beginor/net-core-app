FROM mcr.microsoft.com/dotnet/runtime-deps:5.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_URLS=http://+:5000
ADD dist /net-core-app
VOLUME [ "/net-core-app/config", "/net-core-app/log" ]
WORKDIR /net-core-app
ENTRYPOINT ["./NetCoreApp"]

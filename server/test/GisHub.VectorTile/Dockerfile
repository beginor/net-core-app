FROM mcr.microsoft.com/dotnet/runtime-deps:6.0

ENV ASPNETCORE_PATHBASE=/vector \
    ASPNETCORE_URLS=http://+:5000

ADD bin/Publish/linux-x64 /vector

VOLUME [ "/vector/config", "/vector/log", "/vector/wwwroot" ]
WORKDIR /vector
ENTRYPOINT ["./VectorTile"]

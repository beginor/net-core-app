FROM mcr.microsoft.com/dotnet/runtime-deps:5.0

ENV ASPNETCORE_PATHBASE=/l3a1/vector \
    ASPNETCORE_URLS=http://+:5000

ADD dist /vector

VOLUME [ "/vector/config", "/vector/log" ]
WORKDIR /vector
ENTRYPOINT ["./VectorTile"]

# Build stage =========================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

COPY . /src

#===== Build Middleware
WORKDIR /src/server/horizon-server-database-middleware/Horizon.Database/
RUN dotnet publish -c Release -o /middleware

RUN cp /middleware/*.dll /src/

#====== Build Plugin
WORKDIR /src/
RUN dotnet publish -c Release -o /out/plugin

CMD "/src/entrypoint.sh"

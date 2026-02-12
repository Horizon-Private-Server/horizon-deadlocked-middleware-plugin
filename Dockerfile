# Build stage =========================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

COPY . /src

#===== Build Middleware
WORKDIR /src/middleware/Horizon.Database/
RUN dotnet publish -c Release -o /middleware

RUN cp /middleware/*.dll /src/Horizon.Middleware.Plugin.Deadlocked/

#====== Build Plugin
WORKDIR /src/Horizon.Middleware.Plugin.Deadlocked
RUN dotnet publish -c Release -o /out/plugin
RUN rm -rf /out/plugin/runtimes

CMD "/src/entrypoint.sh"

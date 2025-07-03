# Build React app
FROM node:18-alpine AS client-build
WORKDIR /client
COPY ["client/package*.json", "./"]
RUN npm ci
COPY ["client/", "./"]
RUN npm run build

# Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["server/", "."]
RUN dotnet publish "api/api.csproj" \
    -c Release \
    -o /app/publish \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishSingleFile=true

# Copy React build output to wwwroot
COPY --from=client-build /client/dist /app/publish/wwwroot

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["./api"]
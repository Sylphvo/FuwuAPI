# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# restore theo cache
COPY src/WebApi/WebApi.csproj src/WebApi/
RUN dotnet restore src/WebApi/WebApi.csproj

# copy code và publish
COPY . .
RUN dotnet publish src/WebApi/WebApi.csproj -c Release -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /out .

# Railway sẽ set PORT. Program.cs đã đọc biến PORT và tự bind.
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApi.dll"]

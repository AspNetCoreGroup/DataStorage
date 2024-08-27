FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /DataStorage
COPY ./ ./
RUN dotnet restore "DataStorage.sln"
RUN dotnet build "DataStorage.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataStorageWebApi/DataStorageWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
CMD dotnet DataStorageWebApi.dll
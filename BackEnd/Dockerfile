#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY ["./WebCrawler.BusinessLayer/WebCrawler.BusinessLayer.csproj", "WebCrawler.BusinessLayer/"]
COPY ["./WebCrawler.DataAccessLayer/WebCrawler.DataAccessLayer.csproj", "WebCrawler.DataAccessLayer/"]
COPY ["./WebCrawler.Api/WebCrawler.Api.csproj", "WebCrawler.Api/"]

RUN dotnet restore "WebCrawler.Api/WebCrawler.Api.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/WebCrawler.Api"
RUN dotnet build "WebCrawler.Api.csproj" -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish "WebCrawler.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "WebCrawler.Api.dll" ]
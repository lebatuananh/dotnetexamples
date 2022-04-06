FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/github/GithubTrending.Api/GithubTrending.Api.csproj", "GithubTrending.Api/"]
RUN dotnet restore "src/github/GithubTrending.Api/GithubTrending.Api.csproj"
COPY . .
WORKDIR "/src/GithubTrending.Api"
RUN dotnet build "GithubTrending.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GithubTrending.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GithubTrending.Api.dll"]

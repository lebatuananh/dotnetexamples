FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/icedtea/IcedTea.Api/IcedTea.Api.csproj", "src/icedtea/IcedTea.Api/"]
RUN dotnet restore "src/icedtea/IcedTea.Api/IcedTea.Api.csproj"
COPY . .
WORKDIR "/src/src/icedtea/IcedTea.Api"
RUN dotnet build "IcedTea.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IcedTea.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IcedTea.Api.dll"]

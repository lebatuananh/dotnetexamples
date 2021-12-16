FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/filemanagement/FileManagement/FileManagement.csproj", "src/filemanagement/FileManagement/"]
RUN dotnet restore "src/filemanagement/FileManagement/FileManagement.csproj"
COPY . .
WORKDIR "/src/src/filemanagement/FileManagement"
RUN dotnet build "FileManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileManagement.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileManagement.dll"]
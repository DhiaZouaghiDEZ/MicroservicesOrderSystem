FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj files and restore
COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["InventoryService.Contracts/InventoryService.Contracts.csproj", "InventoryService.Contracts/"]

RUN dotnet restore "InventoryService/InventoryService.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
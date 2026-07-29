FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy all relevant .csproj files relative to the Backend folder
COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["OrderService.Contracts/OrderService.Contracts.csproj", "OrderService.Contracts/"]
COPY ["InventoryService.Contracts/InventoryService.Contracts.csproj", "InventoryService.Contracts/"]
COPY ["PaymentService.Contracts/PaymentService.Contracts.csproj", "PaymentService.Contracts/"]

# 2. Restore dependencies (it will find the referenced projects automatically)
RUN dotnet restore "OrderService/OrderService.csproj"

# 3. Copy the rest of the source code from the Backend folder
COPY . .

# 4. Set working directory to the specific service and build
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
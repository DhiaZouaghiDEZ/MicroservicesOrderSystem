FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj files and restore
COPY ["PaymentService/PaymentService.csproj", "PaymentService/"]
COPY ["PaymentService.Contracts/PaymentService.Contracts.csproj", "PaymentService.Contracts/"]

RUN dotnet restore "PaymentService/PaymentService.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/PaymentService"
RUN dotnet build "PaymentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
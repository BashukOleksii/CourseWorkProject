FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InventorySystem-API/InventorySystem-API.csproj", "InventorySystem-API/"]
COPY ["InventorySystem-Shared/InventorySystem-Shared.csproj", "InventorySystem-Shared/"]

RUN dotnet restore "InventorySystem-API/InventorySystem-API.csproj"

COPY . .

WORKDIR "/src/InventorySystem-API"
RUN dotnet publish "InventorySystem-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "InventorySystem-API.dll"]
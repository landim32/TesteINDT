# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ContratacaoService.Api/ContratacaoService.Api.csproj", "ContratacaoService.Api/"]
COPY ["ContratacaoService.Application/ContratacaoService.Application.csproj", "ContratacaoService.Application/"]
COPY ["ContratacaoService.Domain/ContratacaoService.Domain.csproj", "ContratacaoService.Domain/"]
COPY ["ContratacaoService.Infrastructure/ContratacaoService.Infrastructure.csproj", "ContratacaoService.Infrastructure/"]

RUN dotnet restore "ContratacaoService.Api/ContratacaoService.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ContratacaoService.Api"
RUN dotnet build "ContratacaoService.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "ContratacaoService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.Api.dll"]

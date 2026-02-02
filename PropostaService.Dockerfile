# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PropostaService.Api/PropostaService.Api.csproj", "PropostaService.Api/"]
COPY ["PropostaService.Application/PropostaService.Application.csproj", "PropostaService.Application/"]
COPY ["PropostaService.Domain/PropostaService.Domain.csproj", "PropostaService.Domain/"]
COPY ["PropostaService.Infrastructure/PropostaService.Infrastructure.csproj", "PropostaService.Infrastructure/"]

RUN dotnet restore "PropostaService.Api/PropostaService.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PropostaService.Api"
RUN dotnet build "PropostaService.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "PropostaService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropostaService.Api.dll"]

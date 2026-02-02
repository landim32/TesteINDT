# Troubleshooting Guide - ContratacaoService

## Problema: Swagger não está acessível (404)

### Sintomas
- Acessar `https://localhost:7082/swagger` retorna 404
- Log mostra: "Request reached the end of the middleware pipeline without being handled"

### Soluções

#### 1. Verificar Ambiente de Execução
O Swagger só está habilitado em ambiente de **Development**. Verifique:

```bash
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Windows CMD
set ASPNETCORE_ENVIRONMENT=Development

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development
```

#### 2. Usar a Porta HTTP (sem SSL)
Por padrão, o ContratacaoService está configurado para rodar em:
- **HTTP**: http://localhost:5001
- **HTTPS**: https://localhost:5002

Acesse o Swagger via HTTP:
```
http://localhost:5001/swagger
```

#### 3. Executar com Profile Correto
```bash
cd ContratacaoService.Api

# Usar profile HTTP (recomendado para desenvolvimento)
dotnet run --launch-profile http

# Ou especificar a porta diretamente
dotnet run --urls "http://localhost:5001"
```

#### 4. Verificar launchSettings.json
Arquivo: `ContratacaoService.Api/Properties/launchSettings.json`

```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

#### 5. Desabilitar HTTPS Redirection (Desenvolvimento)
Se estiver tendo problemas com HTTPS, você pode comentar temporariamente:

```csharp
// app.UseHttpsRedirection(); // Comentar temporariamente
```

## Problema: Erro de Conexão com Banco de Dados

### Sintomas
- Erro ao executar migrations
- Timeout ao conectar no PostgreSQL

### Soluções

#### 1. Verificar Docker
```bash
# Verificar se os containers estão rodando
docker-compose ps

# Iniciar containers se necessário
docker-compose up -d

# Ver logs do PostgreSQL
docker-compose logs -f postgres
```

#### 2. Verificar Connection String
Arquivo: `ContratacaoService.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "ContratacaoDb": "Host=localhost;Port=5432;Database=ContratacaoServiceDb;Username=postgres;Password=postgres123"
  }
}
```

#### 3. Verificar se o Banco Existe
```bash
# Conectar ao PostgreSQL
docker exec -it propostas-postgres psql -U postgres

# Listar bancos
\l

# Criar banco manualmente se necessário
CREATE DATABASE "ContratacaoServiceDb";
```

#### 4. Executar Migrations Manualmente
```bash
cd ContratacaoService.Api

# Aplicar migrations
dotnet ef database update

# Ou criar nova migration
dotnet ef migrations add NomeDaMigration
```

## Problema: Erro ao Consultar PropostaService

### Sintomas
- Erro 500 ao criar contrato
- "Connection refused" nos logs

### Soluções

#### 1. Verificar se PropostaService está Rodando
```bash
# Deve estar rodando em http://localhost:5000
curl http://localhost:5000/swagger
```

#### 2. Verificar appsettings.json
```json
{
  "ExternalServices": {
    "PropostaService": {
      "BaseUrl": "http://localhost:5000"
    }
  }
}
```

#### 3. Testar Comunicação
```bash
# Criar uma proposta primeiro
curl -X POST http://localhost:5000/api/propostas \
  -H "Content-Type: application/json" \
  -d '{"nomeCliente":"Test","cpf":"12345678909","tipoSeguro":"Auto","valorCobertura":50000,"valorPremio":1200}'

# Aprovar a proposta (pegue o ID retornado)
curl -X PATCH http://localhost:5000/api/propostas/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status":2}'

# Criar contrato
curl -X POST http://localhost:5001/api/contratos \
  -H "Content-Type: application/json" \
  -d '{"propostaId":"ID_DA_PROPOSTA"}'
```

## Problema: RabbitMQ não está acessível

### Sintomas
- Erro ao publicar/consumir mensagens
- Connection refused

### Soluções

#### 1. Verificar RabbitMQ
```bash
# Ver logs
docker-compose logs -f rabbitmq

# Acessar management console
# http://localhost:15672
# Usuário: guest
# Senha: guest
```

#### 2. Verificar appsettings.json
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": "5672",
    "Username": "guest",
    "Password": "guest"
  }
}
```

## Comandos Úteis

### Limpar e Reiniciar Tudo
```bash
# Parar aplicações
# CTRL+C em cada terminal

# Limpar containers e volumes
docker-compose down -v

# Limpar build
dotnet clean

# Reiniciar containers
docker-compose up -d

# Aguardar containers ficarem healthy
docker-compose ps

# Executar migrations
cd PropostaService.Api
dotnet ef database update

cd ../ContratacaoService.Api
dotnet ef database update

# Executar aplicações
cd ../PropostaService.Api
dotnet run --launch-profile http

# Em outro terminal
cd ../ContratacaoService.Api
dotnet run --launch-profile http
```

### Verificar Logs da Aplicação
```bash
# Ver logs em tempo real
dotnet run --launch-profile http

# Com mais detalhes
dotnet run --launch-profile http --verbosity detailed
```

### Testar Endpoints
```bash
# PropostaService Swagger
curl http://localhost:5000/swagger

# ContratacaoService Swagger
curl http://localhost:5001/swagger

# Health checks
curl http://localhost:5000/api/propostas
curl http://localhost:5001/api/contratos
```

## Portas Utilizadas

| Serviço | Porta | URL |
|---------|-------|-----|
| PropostaService (HTTP) | 5000 | http://localhost:5000 |
| PropostaService (HTTPS) | 5001 | https://localhost:5001 |
| ContratacaoService (HTTP) | 5001 | http://localhost:5001 |
| ContratacaoService (HTTPS) | 5002 | https://localhost:5002 |
| PostgreSQL | 5432 | localhost:5432 |
| RabbitMQ | 5672 | localhost:5672 |
| RabbitMQ Management | 15672 | http://localhost:15672 |

**Nota**: Para evitar conflitos de porta, execute cada serviço em terminais separados e use as portas HTTP (5000 e 5001).

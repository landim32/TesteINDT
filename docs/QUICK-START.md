# ?? Guia Rápido - Comandos Essenciais

## ?? Primeira Execução ou Erro de Imagem

Se você receber o erro:
```
WARNING: The DOCKER_REGISTRY variable is not set
ERROR: The image for the service you're trying to recreate has been removed
```

**Solução:**
```bash
# 1. Limpar containers e volumes antigos
docker-compose down -v

# 2. Build e iniciar (força rebuild)
docker-compose up -d --build

# 3. Ver logs
docker-compose logs -f
```

## Início Rápido

### Opção 1: Tudo em Docker (Recomendado para testes)

```bash
# 1. Limpar ambiente (se necessário)
docker-compose down -v

# 2. Build e iniciar tudo
docker-compose up -d --build

# 3. Aguardar containers ficarem healthy
docker-compose ps

# 4. Acessar
# Swagger: http://localhost:5000/swagger
# Health: http://localhost:5000/health
# RabbitMQ: http://localhost:15672
```

### Opção 2: Desenvolvimento Local (Recomendado para desenvolvimento)

```bash
# 1. Configurar ambiente
cp .env.example .env

# 2. Iniciar apenas infraestrutura
docker-compose up -d postgres rabbitmq

# 3. Aguardar serviços (10 segundos)
Start-Sleep -Seconds 10  # Windows
sleep 10                  # Linux/Mac

# 4. Aplicar migrations
cd PropostaService.Api
dotnet ef database update
cd ..

# 5. Executar API localmente
dotnet run --project PropostaService.Api --launch-profile http

# OU use os scripts:
# Windows: .\start.ps1
# Linux/Mac: ./start.sh
```

## ?? Docker - Comandos Essenciais

### Iniciar e Parar
```bash
# Iniciar todos os serviços
docker-compose up -d

# Iniciar apenas infraestrutura
docker-compose up -d postgres rabbitmq

# Parar (dados preservados)
docker-compose stop

# Parar e remover containers (dados preservados)
docker-compose down
```

### Ver Status e Logs
```bash
# Ver status
docker-compose ps

# Ver logs de todos
docker-compose logs -f

# Ver logs específicos
docker-compose logs -f proposta-service
docker-compose logs -f postgres
docker-compose logs -f rabbitmq
```

### Rebuild Após Mudanças
```bash
# Rebuild apenas PropostaService
docker-compose up -d --build proposta-service

# Rebuild tudo
docker-compose up -d --build
```

### Limpar Tudo (?? APAGA DADOS)
```bash
# Remover containers e volumes
docker-compose down -v

# Reconstruir do zero
docker-compose up -d --build
```

## ?? Verificação de Saúde

```bash
# Status dos containers
docker-compose ps

# Health check da API
curl http://localhost:5000/health

# Testar Swagger
curl http://localhost:5000/swagger

# Ver logs em tempo real
docker-compose logs -f proposta-service
```

## ?? URLs Importantes

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **API (Docker)** | http://localhost:5000 | - |
| **Swagger (Docker)** | http://localhost:5000/swagger | - |
| **Health Check (Docker)** | http://localhost:5000/health | - |
| **API (Local)** | http://localhost:5000 | - |
| **PostgreSQL** | localhost:5432 | postgres/postgres123 |
| **RabbitMQ Management** | http://localhost:15672 | guest/guest |
| **RabbitMQ AMQP** | localhost:5672 | guest/guest |

## ?? Problemas Comuns

### Erro: "port is already allocated"
```bash
# Verificar o que está usando a porta
netstat -ano | findstr :5000

# Ou mudar a porta no docker-compose.yml
# de "5000:80" para "5001:80"
```

### Container fica reiniciando
```bash
# Ver logs detalhados
docker-compose logs proposta-service

# Verificar se PostgreSQL e RabbitMQ estão healthy
docker-compose ps
```

### Banco de dados não conecta
```bash
# Verificar se PostgreSQL está rodando
docker-compose ps postgres

# Ver logs
docker-compose logs postgres

# Testar conexão
docker exec -it propostas-postgres psql -U postgres -c "\l"
```

### Build está lento ou travado
```bash
# Limpar cache de build
docker builder prune -a

# Rebuild sem cache
docker-compose build --no-cache
docker-compose up -d
```

## ?? Workflow de Desenvolvimento

### Para Desenvolvimento Ativo (Hot Reload)
```bash
# 1. Infraestrutura em Docker
docker-compose up -d postgres rabbitmq

# 2. API Local com hot reload
cd PropostaService.Api
dotnet watch run --launch-profile http
```

### Para Testar em Ambiente Containerizado
```bash
# 1. Fazer alterações no código

# 2. Rebuild e restart
docker-compose up -d --build proposta-service

# 3. Ver logs
docker-compose logs -f proposta-service
```

## ?? Banco de Dados

### Conectar ao PostgreSQL
```bash
# Via Docker
docker exec -it propostas-postgres psql -U postgres

# Listar bancos
\l

# Conectar ao PropostaServiceDb
\c PropostaServiceDb

# Ver tabelas
\dt
```

### Backup e Restore
```bash
# Backup
docker exec -t propostas-postgres pg_dump -U postgres PropostaServiceDb > backup.sql

# Restore
docker exec -i propostas-postgres psql -U postgres PropostaServiceDb < backup.sql
```

## ?? Documentação Completa

- [README.md](README.md) - Visão geral do projeto
- [DOCKER-COMPOSE.md](DOCKER-COMPOSE.md) - Guia completo do Docker
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Solução de problemas detalhada
- [USER-SECRETS.md](USER-SECRETS.md) - Gerenciamento de secrets
- [RABBITMQ.md](RABBITMQ.md) - Configuração do RabbitMQ

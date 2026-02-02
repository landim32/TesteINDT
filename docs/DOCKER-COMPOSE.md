# Docker Compose - PropostaService

## ?? Estrutura de Containers

O sistema agora roda completamente em containers Docker:

| Container | Porta Host | Porta Container | Descrição |
|-----------|-----------|-----------------|-----------|
| **proposta-service** | 5000 | 80 | API do PropostaService |
| **postgres** | 5432 | 5432 | PostgreSQL (2 bancos) |
| **rabbitmq** | 5672, 15672 | 5672, 15672 | RabbitMQ + Management |

## ?? Como Usar

### 1. Iniciar Todos os Serviços

```bash
# Iniciar em background
docker-compose up -d

# Ver logs em tempo real
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f proposta-service
```

### 2. Verificar Status

```bash
# Ver status dos containers
docker-compose ps

# Ver logs
docker-compose logs proposta-service
```

### 3. Acessar os Serviços

#### PropostaService API
- **Swagger**: http://localhost:5000/swagger
- **API**: http://localhost:5000/api/propostas
- **Health Check**: http://localhost:5000/health

#### PostgreSQL
```bash
# Conectar via psql
docker exec -it propostas-postgres psql -U postgres

# Listar bancos
docker exec -it propostas-postgres psql -U postgres -c "\l"

# Conectar ao PropostaServiceDb
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb
```

#### RabbitMQ Management
- **URL**: http://localhost:15672
- **Usuário**: guest
- **Senha**: guest

### 4. Reconstruir e Reiniciar

```bash
# Rebuild da imagem (após mudanças no código)
docker-compose build proposta-service

# Rebuild e restart
docker-compose up -d --build proposta-service

# Rebuild completo de tudo
docker-compose up -d --build
```

### 5. Parar e Remover

```bash
# Parar containers
docker-compose stop

# Parar e remover containers
docker-compose down

# Parar, remover containers e volumes (APAGA DADOS DO BANCO!)
docker-compose down -v
```

## ?? Variáveis de Ambiente

As variáveis são definidas no arquivo `.env`:

```env
# PostgreSQL
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_DB=PropostaServiceDb
POSTGRES_PORT=5432

# ContratacaoService Database
CONTRATACAO_POSTGRES_DB=ContratacaoServiceDb

# RabbitMQ
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_VHOST=/
RABBITMQ_PORT=5672
RABBITMQ_MANAGEMENT_PORT=15672
```

## ?? Configuração da API no Container

A API no container usa as seguintes configurações:

### Connection String
```
Host=postgres;Port=5432;Database=PropostaServiceDb;Username=postgres;Password=postgres123
```

**Nota**: O host é `postgres` (nome do serviço) ao invés de `localhost`.

### RabbitMQ
```
Host=rabbitmq;Port=5672;Username=guest;Password=guest
```

## ?? Troubleshooting

### Container não inicia

```bash
# Ver logs detalhados
docker-compose logs proposta-service

# Ver logs do build
docker-compose build --no-cache proposta-service
```

### Erro de conexão com banco

```bash
# Verificar se o PostgreSQL está healthy
docker-compose ps

# Ver logs do PostgreSQL
docker-compose logs postgres

# Verificar conectividade
docker exec -it proposta-service ping postgres
```

### Erro de conexão com RabbitMQ

```bash
# Verificar se RabbitMQ está healthy
docker-compose ps

# Ver logs do RabbitMQ
docker-compose logs rabbitmq

# Acessar management console
open http://localhost:15672
```

### Reconstruir do zero

```bash
# Remover tudo
docker-compose down -v
docker system prune -a

# Rebuild
docker-compose up -d --build
```

## ?? Monitoramento

### Verificar Health

```bash
# Health check da API
curl http://localhost:5000/health

# Health check do PostgreSQL
docker exec propostas-postgres pg_isready -U postgres

# Health check do RabbitMQ
docker exec propostas-rabbitmq rabbitmq-diagnostics ping
```

### Ver Recursos Utilizados

```bash
# Ver uso de recursos
docker stats

# Ver uso de um container específico
docker stats proposta-service
```

## ?? Workflow de Desenvolvimento

### Desenvolvimento Local (sem Docker)
```bash
cd PropostaService.Api
dotnet run --launch-profile http
```

### Desenvolvimento com Docker
```bash
# Fazer alterações no código
# Rebuild e restart
docker-compose up -d --build proposta-service

# Ver logs
docker-compose logs -f proposta-service
```

### Testar Migrations

```bash
# Entrar no container
docker exec -it proposta-service bash

# Executar migrations
dotnet ef database update

# Ou fazer rollback
dotnet ef database update 0
```

## ?? Backup e Restore

### Backup do Banco

```bash
# Backup do PropostaServiceDb
docker exec -t propostas-postgres pg_dump -U postgres PropostaServiceDb > backup_proposta.sql

# Backup do ContratacaoServiceDb
docker exec -t propostas-postgres pg_dump -U postgres ContratacaoServiceDb > backup_contratacao.sql

# Backup de todos os bancos
docker exec -t propostas-postgres pg_dumpall -U postgres > backup_all.sql
```

### Restore do Banco

```bash
# Restore do PropostaServiceDb
docker exec -i propostas-postgres psql -U postgres PropostaServiceDb < backup_proposta.sql

# Restore do ContratacaoServiceDb
docker exec -i propostas-postgres psql -U postgres ContratacaoServiceDb < backup_contratacao.sql
```

## ?? Comandos Úteis

```bash
# Ver todas as imagens
docker images

# Ver todos os containers (incluindo parados)
docker ps -a

# Remover imagem
docker rmi proposta-service

# Limpar cache de build
docker builder prune

# Ver logs de build
docker-compose build --no-cache --progress=plain proposta-service

# Executar comando no container
docker exec -it proposta-service bash

# Copiar arquivo do container
docker cp proposta-service:/app/appsettings.json .

# Ver variáveis de ambiente do container
docker exec proposta-service env
```

## ?? Network

Os containers estão na mesma rede `propostas-network`, permitindo comunicação entre eles:

```bash
# Inspecionar a rede
docker network inspect propostas-network

# Testar conectividade entre containers
docker exec proposta-service ping postgres
docker exec proposta-service ping rabbitmq
```

## ?? Notas Importantes

1. **Migrations**: As migrations são executadas automaticamente quando a API inicia no container
2. **Health Checks**: O Docker espera os serviços ficarem healthy antes de iniciar dependentes
3. **Volumes**: Os dados do PostgreSQL e RabbitMQ são persistidos em volumes Docker
4. **Rebuild**: Sempre faça rebuild após alterações no código: `docker-compose up -d --build`
5. **Logs**: Use `docker-compose logs -f` para ver logs em tempo real

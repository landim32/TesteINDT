# ?? Guia Docker - Sistema de Propostas de Seguro

Este guia detalha como usar Docker e Docker Compose para executar o banco de dados PostgreSQL e o message broker RabbitMQ.

## ?? Pré-requisitos

- Docker instalado ([Instalar Docker](https://docs.docker.com/get-docker/))
- Docker Compose instalado ([Instalar Docker Compose](https://docs.docker.com/compose/install/))

## ?? Início Rápido

### 1. Configure o arquivo .env

```bash
# Copiar o arquivo de exemplo
cp .env.example .env

# Editar o arquivo com suas configurações
# Usuário Windows: notepad .env
# Usuário Linux/Mac: nano .env
```

### 2. Inicie os containers

```bash
# Iniciar em modo detached (background)
docker-compose up -d

# Ou iniciar e ver logs em tempo real
docker-compose up
```

### 3. Verifique o status

```bash
docker-compose ps
```

## ?? Serviços Disponíveis

### PostgreSQL
- **Porta**: 5432 (configurável via .env)
- **Container**: propostas-postgres
- **Imagem**: postgres:16-alpine
- **Volume**: postgres_data (persistência de dados)

### RabbitMQ
- **Porta AMQP**: 5672 (configurável via .env)
- **Porta Management**: 15672 (configurável via .env)
- **Container**: propostas-rabbitmq
- **Imagem**: rabbitmq:3.13-management-alpine
- **Volumes**: 
  - rabbitmq_data (persistência de dados)
  - rabbitmq_logs (logs)

## ?? Comandos Úteis

### Gerenciamento de Containers

```bash
# Iniciar containers
docker-compose up -d

# Parar containers
docker-compose stop

# Reiniciar containers
docker-compose restart

# Parar e remover containers
docker-compose down

# Parar e remover containers + volumes (?? apaga dados!)
docker-compose down -v

# Ver logs de todos os serviços
docker-compose logs -f

# Ver logs apenas do PostgreSQL
docker-compose logs -f postgres

# Ver logs apenas do RabbitMQ
docker-compose logs -f rabbitmq
```

### Acesso ao PostgreSQL

```bash
# Conectar ao PostgreSQL via psql
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb

# Conectar como outro usuário
docker exec -it propostas-postgres psql -U seu_usuario -d PropostaServiceDb

# Listar bancos de dados
docker exec -it propostas-postgres psql -U postgres -c "\l"

# Listar schemas
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb -c "\dn"

# Listar tabelas
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb -c "\dt propostas.*"
```

### Acesso ao RabbitMQ

```bash
# Acessar o shell do RabbitMQ
docker exec -it propostas-rabbitmq /bin/sh

# Listar filas
docker exec -it propostas-rabbitmq rabbitmqctl list_queues

# Listar exchanges
docker exec -it propostas-rabbitmq rabbitmqctl list_exchanges

# Listar bindings
docker exec -it propostas-rabbitmq rabbitmqctl list_bindings

# Listar connections
docker exec -it propostas-rabbitmq rabbitmqctl list_connections

# Status do RabbitMQ
docker exec -it propostas-rabbitmq rabbitmqctl status
```

### Backup e Restore

```bash
# Fazer backup do banco de dados
docker exec -t propostas-postgres pg_dump -U postgres -F c PropostaServiceDb > backup_$(date +%Y%m%d_%H%M%S).dump

# Fazer backup em SQL
docker exec -t propostas-postgres pg_dump -U postgres PropostaServiceDb > backup_$(date +%Y%m%d_%H%M%S).sql

# Restaurar backup (formato custom)
docker exec -i propostas-postgres pg_restore -U postgres -d PropostaServiceDb -c < backup.dump

# Restaurar backup (formato SQL)
docker exec -i propostas-postgres psql -U postgres -d PropostaServiceDb < backup.sql
```

### Manutenção

```bash
# Ver uso de disco dos volumes
docker system df -v

# Limpar recursos não utilizados
docker system prune

# Limpar volumes não utilizados
docker volume prune

# Reconstruir container
docker-compose up -d --build

# Ver informações do container
docker inspect propostas-postgres

# Ver estatísticas de uso (CPU, Memória)
docker stats propostas-postgres
```

## ?? Troubleshooting

### Container não inicia

```bash
# Verificar logs de erro
docker-compose logs postgres

# Verificar portas em uso
netstat -an | grep 5432  # Linux/Mac
netstat -an | findstr 5432  # Windows

# Remover e recriar container
docker-compose down -v
docker-compose up -d
```

### Problemas de conexão

```bash
# Verificar se o container está rodando
docker ps | grep postgres

# Testar conexão de dentro do container
docker exec -it propostas-postgres pg_isready -U postgres

# Verificar rede Docker
docker network ls
docker network inspect testeindtt_propostas-network
```

### Resetar banco de dados

```bash
# ?? ATENÇÃO: Isso apaga todos os dados!

# Parar e remover volumes
docker-compose down -v

# Reiniciar container
docker-compose up -d

# Aguardar inicialização
sleep 10

# Aplicar migrations novamente
cd PropostaService.Api
dotnet ef database update --project ../PropostaService.Infrastructure
```

## ?? Clientes SQL Recomendados

Para acessar o PostgreSQL, você pode usar:

### DBeaver (Recomendado)
- Download: https://dbeaver.io/download/
- Gratuito e open-source
- Suporte a múltiplos bancos de dados

### pgAdmin (Instalação Local)
- Download: https://www.pgadmin.org/download/
- Interface web dedicada ao PostgreSQL

### Azure Data Studio
- Download: https://docs.microsoft.com/sql/azure-data-studio/download
- Cross-platform, da Microsoft

### psql (Linha de Comando)
```bash
# Já incluído no container Docker
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb
```

**Configuração de Conexão:**
- Host: localhost
- Port: 5432
- Database: PropostaServiceDb
- Username: postgres
- Password: (conforme configurado no .env)

## ?? Monitoramento

### Ver logs em tempo real

```bash
# Ver logs do PostgreSQL
docker-compose logs -f postgres

# Últimas 100 linhas
docker-compose logs --tail=100 postgres
```

### Verificar saúde do container

```bash
# Health check do PostgreSQL
docker inspect --format='{{.State.Health.Status}}' propostas-postgres

# Ver histórico de health checks
docker inspect propostas-postgres | grep -A 10 Health
```

## ?? Segurança

### Boas práticas

1. **Nunca commitar o arquivo .env**
   - Já está no .gitignore

2. **Usar senhas fortes**
   - Altere as senhas padrão do .env.example

3. **Limitar acesso às portas**
   - Em produção, não exponha as portas do PostgreSQL

4. **Backup regular**
   - Faça backups automáticos do banco

5. **Atualizar imagens**
   ```bash
   # Verificar atualizações
   docker-compose pull
   
   # Atualizar e reiniciar
   docker-compose up -d
   ```

## ?? Ambientes

### Desenvolvimento (Local)
```bash
# Usar .env com configurações locais
docker-compose up -d
```

### Produção
```bash
# Usar variáveis de ambiente do sistema
# ou arquivo .env.production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## ?? Recursos Adicionais

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

## ? FAQ

**P: Como alterar a porta do PostgreSQL?**
R: Edite a variável `POSTGRES_PORT` no arquivo .env

**P: Como conectar de outra máquina?**
R: Use o IP da máquina host e a porta configurada (padrão 5432)

**P: Os dados são persistidos?**
R: Sim, usando Docker volumes. Para apagar, use `docker-compose down -v`

**P: Como acessar o banco de uma aplicação externa?**
R: Use a connection string: `Host=localhost;Port=5432;Database=PropostaServiceDb;Username=postgres;Password=suasenha`

**P: Preciso do PgAdmin?**
R: Não, você pode usar qualquer cliente SQL como DBeaver, Azure Data Studio ou pgAdmin instalado localmente.

**P: Como acessar a interface do RabbitMQ?**
R: Acesse `http://localhost:15672` no seu navegador (padrão: usuário `guest`, senha `guest`).

**P: Como alterar as credenciais do RabbitMQ?**
R: Altere as variáveis `RABBITMQ_DEFAULT_USER` e `RABBITMQ_DEFAULT_PASS` no arquivo .env.

# 🏥 Sistema de Propostas de Seguro

Sistema de gerenciamento de propostas de seguro desenvolvido com .NET 8, seguindo Arquitetura Hexagonal (Ports & Adapters), DDD, SOLID e Clean Code.

## 🚀 Início Rápido - Configuração em 3 Passos

### 1️⃣ Configurar Variáveis de Ambiente
```bash
# Copie o arquivo de exemplo
cp .env.example .env
```

### 2️⃣ Iniciar Todos os Serviços com Docker
```bash
# Inicia PostgreSQL, RabbitMQ e ambos os microserviços
docker-compose up -d

# Aguarde os containers iniciarem (cerca de 30 segundos)
docker-compose ps
```

### 3️⃣ Executar Migrations
```bash
# Proposta Service - Criar banco e tabelas
docker exec -it proposta-service dotnet ef database update --project /src/PropostaService.Infrastructure

# Contratacao Service - Criar banco e tabelas
docker exec -it contratacao-service dotnet ef database update --project /src/ContratacaoService.Infrastructure
```

### ✅ Pronto! Acesse os Serviços

| Serviço | URL | Descrição |
|---------|-----|-----------|
| 📋 **PropostaService API** | http://localhost:5000/swagger | Swagger UI - Gestão de Propostas |
| 📝 **ContratacaoService API** | http://localhost:5001/swagger | Swagger UI - Gestão de Contratos |
| 🐰 **RabbitMQ Management** | http://localhost:15672 | Interface de gerenciamento (guest/guest) |
| 🗄️ **PostgreSQL** | localhost:5432 | Banco de dados (postgres/postgres123) |

### 📬 Collection do Postman

Importe a collection pronta para testar a API:
- Arquivo: `postman_collection.json` (na raiz do projeto)
- Contém todos os endpoints configurados
- Inclui exemplos de requests
- Variáveis de ambiente pré-configuradas

---

## 🏗️ Arquitetura de Microserviços

O sistema é composto por dois microserviços independentes:

### 1. **PropostaService** (Porta 5000)
Gerencia o ciclo de vida das propostas de seguro:
- ✅ Criar propostas
- 📋 Listar propostas
- 🔄 Atualizar status (Em Análise, Aprovada, Rejeitada)
- 📤 Publicar eventos no RabbitMQ

### 2. **ContratacaoService** (Porta 5001)
Gerencia a contratação de seguros aprovados:
- 📝 Criar contratos para propostas aprovadas
- 🔍 Consultar status de propostas via HTTP
- 📥 Consumir eventos do RabbitMQ
- 🔄 Implementa **Saga Pattern** para transações distribuídas

## 📊 Diagrama de Arquitetura

![Arquitetura do Sistema](diagrama.png)

## 🛠️ Tecnologias

- **.NET 8** - Framework principal
- **PostgreSQL 16** - 1 instância com 2 bancos de dados isolados
- **RabbitMQ 3.13** - Mensageria assíncrona
- **Entity Framework Core** - ORM
- **MediatR** - CQRS + Mediator Pattern
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de entrada
- **xUnit + Moq + FluentAssertions** - Testes (257 testes no total)
- **Docker & Docker Compose** - Containerização

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## 🐳 Docker Compose - Guia Completo

### Configurar Variáveis de Ambiente

Edite o arquivo `.env` com suas configurações:

```env
# PostgreSQL Configuration (único servidor para ambos os bancos)
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_DB=PropostaServiceDb
POSTGRES_PORT=5432

# RabbitMQ Configuration
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_VHOST=/
RABBITMQ_PORT=5672
RABBITMQ_MANAGEMENT_PORT=15672
```

### 🎯 Comandos Essenciais

```bash
# ⬆️ Iniciar todos os serviços
docker-compose up -d

# 📊 Ver status dos containers
docker-compose ps

# 📜 Ver logs em tempo real
docker-compose logs -f

# 📜 Ver logs de um serviço específico
docker-compose logs -f proposta-service
docker-compose logs -f contratacao-service

# 🔄 Rebuild após alterações no código
docker-compose up -d --build

# ⏹️ Parar containers (mantém dados)
docker-compose stop

# 🗑️ Parar e remover containers
docker-compose down

# ⚠️ Remover tudo incluindo volumes (apaga dados!)
docker-compose down -v
```

### 🔧 Modo Desenvolvimento Local

Se preferir executar as APIs localmente (sem Docker):

```bash
# 1️⃣ Iniciar apenas infraestrutura
docker-compose up -d postgres rabbitmq

# 2️⃣ Executar PropostaService
cd PropostaService.Api
dotnet run

# 3️⃣ Em outro terminal, executar ContratacaoService
cd ContratacaoService.Api
dotnet run
```

## 📡 Endpoints da API

### PropostaService (http://localhost:5000)

#### 📋 Propostas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/propostas` | Criar nova proposta |
| GET | `/api/propostas` | Listar todas as propostas |
| GET | `/api/propostas/{id}` | Obter proposta por ID |
| PUT | `/api/propostas/{id}` | Atualizar proposta |
| PATCH | `/api/propostas/{id}/status` | Alterar status da proposta |

#### 📝 Exemplo de Request - Criar Proposta

```json
{
  "nomeCliente": "João da Silva",
  "cpf": "123.456.789-09",
  "tipoSeguro": "Auto",
  "valorCobertura": 50000.00,
  "valorPremio": 1200.00
}
```

#### 🔄 Exemplo de Request - Alterar Status

```json
{
  "status": 2  // 1=EmAnalise, 2=Aprovada, 3=Rejeitada
}
```

### ContratacaoService (http://localhost:5001)

#### 📝 Contratos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/contratos` | Criar contrato (requer proposta aprovada) |
| GET | `/api/contratos` | Listar todos os contratos |
| GET | `/api/contratos/{id}` | Obter contrato por ID |

#### 📝 Exemplo de Request - Criar Contrato

```json
{
  "propostaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## 🔗 Fluxo de Comunicação entre Microserviços

### 1. 🔄 Comunicação Síncrona (HTTP)
O ContratacaoService consulta o PropostaService via HTTP para verificar o status da proposta antes de criar um contrato.

### 2. 📨 Comunicação Assíncrona (RabbitMQ)
- **PropostaService** publica eventos quando uma proposta é aprovada/rejeitada
- **ContratacaoService** consome esses eventos para processar automaticamente

### 3. 🔄 Saga Pattern
O ContratacaoService implementa o Saga Pattern para gerenciar transações distribuídas:
1. **VerificarPropostaStep**: Consulta status da proposta
2. **CriarContratoStep**: Cria o contrato no banco de dados
3. **NotificarContratoStep**: Publica evento de contrato criado
4. **Compensação**: Reverte operações em caso de falha

## 🗄️ Banco de Dados

### PostgreSQL (1 servidor, 2 bancos isolados)

- **Host**: localhost
- **Port**: 5432
- **Username**: postgres
- **Password**: postgres123
- **Databases**: 
  - `PropostaServiceDb` (PropostaService)
  - `ContratacaoServiceDb` (ContratacaoService)

### 🔧 Comandos Úteis do PostgreSQL

```bash
# Acessar o shell do PostgreSQL
docker exec -it propostas-postgres psql -U postgres

# Conectar ao banco PropostaServiceDb
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb

# Conectar ao banco ContratacaoServiceDb
docker exec -it propostas-postgres psql -U postgres -d ContratacaoServiceDb

# Listar todos os bancos de dados
docker exec -it propostas-postgres psql -U postgres -c "\l"

# Ver tabelas do PropostaServiceDb
docker exec -it propostas-postgres psql -U postgres -d PropostaServiceDb -c "\dt"

# Backup do banco
docker exec -t propostas-postgres pg_dump -U postgres PropostaServiceDb > backup.sql

# Restaurar backup
docker exec -i propostas-postgres psql -U postgres PropostaServiceDb < backup.sql
```

### 🔌 Clientes Recomendados

- **DBeaver** (Recomendado) - https://dbeaver.io/
- **pgAdmin** - https://www.pgadmin.org/
- **Azure Data Studio** - https://docs.microsoft.com/sql/azure-data-studio/
- **psql** (linha de comando)

## 🐰 RabbitMQ

### 🔗 Acesso

- **Management Console**: http://localhost:15672
- **Username**: guest
- **Password**: guest
- **AMQP Port**: 5672

### 📬 Exchanges e Queues

- **proposta.events** - Exchange para eventos de propostas
- **contrato.events** - Exchange para eventos de contratos
- **proposta.criada** - Queue para propostas criadas
- **proposta.aprovada** - Queue para propostas aprovadas
- **contrato.criado** - Queue para contratos criados

## 🧪 Executar Testes

### ✅ PropostaService.Tests - 186 testes
```bash
# Executar todos os testes
dotnet test PropostaService.Tests

# Com cobertura
dotnet test PropostaService.Tests /p:CollectCoverage=true

# Com output detalhado
dotnet test PropostaService.Tests --verbosity normal
```

### ✅ ContratacaoService.Tests - 71 testes
```bash
# Executar todos os testes
dotnet test ContratacaoService.Tests

# Com cobertura
dotnet test ContratacaoService.Tests /p:CollectCoverage=true
```

### 🎯 Executar Todos os Testes
```bash
# Total: 257 testes (186 + 71)
dotnet test

# Com relatório de cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### 📊 Cobertura de Testes

- **PropostaService**: 186 testes (100% de sucesso)
  - 75 testes de domínio
  - 79 testes de aplicação
  - 32 testes de infraestrutura

- **ContratacaoService**: 71 testes (100% de sucesso)
  - 47 testes de domínio
  - 12 testes de aplicação
  - 12 testes de infraestrutura

## 🏗️ Arquitetura e Padrões

### 🔷 Arquitetura Hexagonal (Ports & Adapters)
- **Núcleo (Domain)**: Regras de negócio puras, independente de frameworks
- **Ports**: Interfaces que definem contratos
- **Adapters**: Implementações concretas (EF Core, API Controllers, RabbitMQ)

### 🎯 Domain-Driven Design (DDD)
- **Entities**: Objetos com identidade única (Proposta, Contrato)
- **Value Objects**: Objetos imutáveis sem identidade (Cpf, Dinheiro, PropostaId)
- **Aggregates**: Proposta e Contrato como Aggregate Roots
- **Domain Events**: Eventos de domínio para comunicação
- **Domain Services**: Lógica que não pertence a uma entidade
- **Specifications**: Regras de negócio reutilizáveis

### 🎨 SOLID Principles
- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

### 🛠️ Design Patterns Implementados
- **Repository Pattern** - Abstração da camada de dados
- **Unit of Work** - Gerenciamento de transações
- **CQRS** - Separação de Commands e Queries
- **Mediator Pattern** - Desacoplamento de handlers (MediatR)
- **Specification Pattern** - Regras de negócio reutilizáveis
- **Saga Pattern** - Transações distribuídas entre microserviços
- **Builder Pattern** - Construção de objetos de teste
- **Pipeline Behavior** - Cross-cutting concerns (validação, logging, transação)

## 🔍 Troubleshooting

### ❌ Swagger retorna 404
✅ **Solução**: Verifique se está usando o perfil HTTP e ambiente Development

### ❌ Erro de conexão com banco
✅ **Solução**: 
```bash
# Verificar se Docker está rodando
docker ps

# Verificar health dos containers
docker-compose ps

# Ver logs do PostgreSQL
docker-compose logs postgres
```

### ❌ Porta em uso
✅ **Solução**: Certifique-se de que as portas estão livres
```bash
# Windows
netstat -ano | findstr :5000
netstat -ano | findstr :5001
netstat -ano | findstr :5432

# Linux/Mac
lsof -i :5000
lsof -i :5001
lsof -i :5432
```

### ❌ Container não inicia
✅ **Solução**: Ver logs detalhados
```bash
docker-compose logs proposta-service
docker-compose logs contratacao-service
```

### ❌ RabbitMQ não conecta
✅ **Solução**: Verificar se o container está healthy
```bash
docker-compose ps rabbitmq
docker-compose logs rabbitmq
```

## 📚 Estrutura do Projeto

```
TesteINDT/
├── 📁 PropostaService.Api/           # API REST do PropostaService
├── 📁 PropostaService.Application/   # Casos de uso, DTOs, Validações
├── 📁 PropostaService.Domain/        # Entidades, Value Objects, Regras de Negócio
├── 📁 PropostaService.Infrastructure/# EF Core, Repositories, RabbitMQ
├── 📁 PropostaService.Tests/         # 186 testes unitários e de integração
├── 📁 ContratacaoService.Api/        # API REST do ContratacaoService
├── 📁 ContratacaoService.Application/# Casos de uso, Saga, DTOs
├── 📁 ContratacaoService.Domain/     # Entidades, Value Objects
├── 📁 ContratacaoService.Infrastructure/# EF Core, Repositories, HTTP Client
├── 📁 ContratacaoService.Tests/      # 71 testes unitários e de integração
├── 🐳 docker-compose.yml             # Orquestração dos containers
├── 🐳 PropostaService.Dockerfile     # Imagem Docker do PropostaService
├── 🐳 ContratacaoService.Dockerfile  # Imagem Docker do ContratacaoService
├── 📄 .env.example                   # Exemplo de variáveis de ambiente
├── 📄 ARQUITETURA.md                 # Documentação completa da arquitetura
├── 📬 postman_collection.json        # Collection do Postman
└── 📖 README.md                      # Este arquivo
```

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.

## 👨‍💻 Autor

Desenvolvido seguindo as melhores práticas de:
- ✅ Clean Code
- ✅ Clean Architecture
- ✅ Domain-Driven Design (DDD)
- ✅ SOLID Principles
- ✅ Arquitetura Hexagonal
- ✅ Microservices Architecture
- ✅ Event-Driven Architecture
- ✅ Test-Driven Development (TDD)

---

⭐ Se este projeto foi útil para você, considere dar uma estrela no GitHub!

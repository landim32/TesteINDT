# Sistema de Propostas de Seguro

Desenvolver um sistema simples que permita gerenciar propostas de seguro e efetuar 
sua contratação, utilizando: 
- Arquitetura Hexagonal (Ports & Adapters) 
- Abordagem baseada em microserviços (APIs) 
- Banco de dados relacional em Postgres
- Use Code First com Entity Framework Core
- Boas práticas de Clean Code, DDD, SOLID, design partners e testes unitários 
- Usar Saga Pattern

## Requisitos Funcionais

PropostaService:
- Criar, ler e atualizar propostas de seguro.
- Cada proposta deve conter informações como: 
	* ID da proposta
	* Nome do cliente
	* CPF
	* Tipo do Seguro
	* Valor da cobretura
	* Valor do Premio
	* Data de criação
	* Status (Em Análise, Aprovada, Rejeitada)
- Na API deve ser possível:
    * Criar proposta de seguro 
    * Listar propostas 
    * Alterar status da proposta (Em Análise, Aprovada, Rejeitada) 
- Use as seguintes bibliotecas:
    * ASP.NET Core Web API
    * Entity Framework Core
    * MediatR para CQRS e Mediator Pattern
    * AutoMapper para mapeamento de objetos
    * FluentValidation para validação de dados
    * xUnit e Moq para testes unitários
ContratacaoService:
- Contratar seguro para propostas aprovadas
- Armazenar contratos no banco de dados:
    * Id da proposta
    * Data de contratação
- Comunicar-se com o PropostaService para verificar status da proposta usando o RabbitMQ
    

## Estrutura de Pastas - Arquitetura Hexagonal

### PropostaService (Microserviço de Propostas)

```
PropostaService/
│
├── PropostaService.Domain/                    # NÚCLEO - Camada de Domínio (Hexágono Central)
│   ├── Entities/                              # Entidades do DDD
│   │   ├── Proposta.cs                        # Aggregate Root
│   │   ├── Cliente.cs                         # Value Object
│   │   └── Seguro.cs                          # Entity
│   │
│   ├── ValueObjects/                          # Value Objects do DDD
│   │   ├── Cpf.cs
│   │   ├── Dinheiro.cs
│   │   └── TipoSeguro.cs
│   │
│   ├── Enums/                                 # Enumerações
│   │   └── StatusProposta.cs
│   │
│   ├── Interfaces/                            # Ports (Portas de Entrada)
│   │   ├── Repositories/                      # Port para Persistência
│   │   │   ├── IPropostaRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Services/                          # Port para Serviços de Domínio
│   │       └── IPropostaValidationService.cs
│   │
│   ├── Services/                              # Domain Services
│   │   └── PropostaValidationService.cs
│   │
│   ├── Specifications/                        # Specification Pattern
│   │   ├── ISpecification.cs
│   │   └── PropostaAprovadaSpecification.cs
│   │
│   ├── Events/                                # Domain Events
│   │   ├── PropostaCriadaEvent.cs
│   │   ├── PropostaAprovadaEvent.cs
│   │   └── PropostaRejeitadaEvent.cs
│   │
│   └── Exceptions/                            # Domain Exceptions
│       ├── DomainException.cs
│       └── PropostaInvalidaException.cs
│
├── PropostaService.Application/               # Camada de Aplicação (Casos de Uso)
│   ├── UseCases/                              # Use Cases / Application Services
│   │   ├── CriarProposta/
│   │   │   ├── CriarPropostaCommand.cs       # Command (CQRS)
│   │   │   ├── CriarPropostaHandler.cs       # Command Handler
│   │   │   └── CriarPropostaValidator.cs     # FluentValidation
│   │   ├── ObterProposta/
│   │   │   ├── ObterPropostaQuery.cs         # Query (CQRS)
│   │   │   └── ObterPropostaHandler.cs       # Query Handler
│   │   ├── AtualizarProposta/
│   │   │   ├── AtualizarPropostaCommand.cs
│   │   │   ├── AtualizarPropostaHandler.cs
│   │   │   └── AtualizarPropostaValidator.cs
│   │   └── AprovarProposta/
│   │       ├── AprovarPropostaCommand.cs
│   │       └── AprovarPropostaHandler.cs
│   │
│   ├── DTOs/                                  # Data Transfer Objects
│   │   ├── PropostaDto.cs
│   │   ├── CriarPropostaDto.cs
│   │   └── AtualizarPropostaDto.cs
│   │
│   ├── Mappings/                              # AutoMapper Profiles
│   │   └── PropostaMappingProfile.cs
│   │
│   ├── Interfaces/                            # Application Ports
│   │   └── IPropostaApplicationService.cs
│   │
│   ├── Behaviors/                             # MediatR Pipeline Behaviors
│   │   ├── ValidationBehavior.cs             # Comportamento de Validação
│   │   ├── LoggingBehavior.cs                # Comportamento de Log
│   │   └── TransactionBehavior.cs            # Comportamento Transacional
│   │
│   └── Services/                              # Application Services
│       └── PropostaApplicationService.cs
│
├── PropostaService.Infrastructure/            # ADAPTADORES - Camada de Infraestrutura
│   ├── Persistence/                           # Adapter para Persistência
│   │   ├── Context/
│   │   │   └── PropostaDbContext.cs          # EF Core DbContext
│   │   ├── Configurations/                    # EF Core Configurations
│   │   │   ├── PropostaConfiguration.cs
│   │   │   └── ClienteConfiguration.cs
│   │   ├── Repositories/                      # Implementação dos Repositories
│   │   │   ├── PropostaRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   └── Migrations/                        # EF Core Migrations
│   │
│   ├── ExternalServices/                      # Adapter para Serviços Externos
│   │   ├── Messaging/                         # Message Broker (RabbitMQ/Kafka)
│   │   │   ├── IMessagePublisher.cs
│   │   │   └── RabbitMqPublisher.cs
│   │   └── HttpClients/                       # HTTP Clients
│   │       └── ContratacaoServiceClient.cs
│   │
│   ├── CrossCutting/                          # Concerns Transversais
│   │   ├── Logging/
│   │   │   └── SerilogConfiguration.cs
│   │   ├── Caching/
│   │   │   └── RedisCacheService.cs
│   │   └── Security/
│   │       └── JwtConfiguration.cs
│   │
│   └── DependencyInjection/                   # IoC Container Configuration
│       └── InfrastructureModule.cs
│
├── PropostaService.Api/                       # ADAPTADOR - Camada de Apresentação (REST API)
│   ├── Controllers/                           # REST Controllers
│   │   └── PropostasController.cs
│   │
│   ├── Middlewares/                           # Custom Middlewares
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── LoggingMiddleware.cs
│   │   └── AuthenticationMiddleware.cs
│   │
│   ├── Filters/                               # Action Filters
│   │   ├── ValidateModelAttribute.cs
│   │   └── ApiExceptionFilterAttribute.cs
│   │
│   ├── ViewModels/                            # Request/Response Models
│   │   ├── Request/
│   │   │   ├── CriarPropostaRequest.cs
│   │   │   └── AtualizarPropostaRequest.cs
│   │   └── Response/
│   │       ├── PropostaResponse.cs
│   │       └── ApiErrorResponse.cs
│   │
│   ├── Configuration/                         # API Configuration
│   │   ├── SwaggerConfiguration.cs
│   │   ├── CorsConfiguration.cs
│   │   └── ApiVersioningConfiguration.cs
│   │
│   ├── Program.cs                             # Application Entry Point
│   └── appsettings.json                       # Configuration Files
│
└── PropostaService.Tests/                     # Camada de Testes
    ├── Unit/                                  # Testes Unitários
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   └── PropostaTests.cs
    │   │   ├── ValueObjects/
    │   │   │   └── CpfTests.cs
    │   │   └── Services/
    │   │       └── PropostaValidationServiceTests.cs
    │   └── Application/
    │       └── UseCases/
    │           └── CriarPropostaHandlerTests.cs
    │
    ├── Integration/                           # Testes de Integração
    │   ├── Api/
    │   │   └── PropostasControllerTests.cs
    │   └── Infrastructure/
    │       └── PropostaRepositoryTests.cs
    │
    ├── Fixtures/                              # Test Fixtures
    │   └── PropostaFixture.cs
    │
    └── Builders/                              # Test Builders (Builder Pattern)
        └── PropostaBuilder.cs
```

### ContratacaoService (Microserviço de Contratação)

```
ContratacaoService/
│
├── ContratacaoService.Domain/                 # NÚCLEO - Camada de Domínio (Hexágono Central)
│   ├── Entities/                              # Entidades do DDD
│   │   ├── Contrato.cs                        # Aggregate Root
│   │   └── Proposta.cs                        # Entity (referência externa)
│   │
│   ├── ValueObjects/                          # Value Objects do DDD
│   │   └── PropostaId.cs
│   │
│   ├── Enums/                                 # Enumerações
│   │   ├── StatusContrato.cs
│   │   └── StatusProposta.cs
│   │
│   ├── Interfaces/                            # Ports (Portas de Entrada)
│   │   ├── Repositories/                      # Port para Persistência
│   │   │   ├── IContratoRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Services/                          # Port para Serviços de Domínio
│   │       ├── IContratoValidationService.cs
│   │       └── IPropostaValidationService.cs
│   │
│   ├── Services/                              # Domain Services
│   │   ├── ContratoValidationService.cs
│   │   └── PropostaValidationService.cs
│   │
│   ├── Specifications/                        # Specification Pattern
│   │   ├── ISpecification.cs
│   │   ├── PropostaAprovadaSpecification.cs
│   │   └── ContratoAtivoSpecification.cs
│   │
│   ├── Events/                                # Domain Events
│   │   ├── ContratoCriadoEvent.cs
│   │   ├── ContratoAprovadoEvent.cs
│   │   └── ContratoRejeitadoEvent.cs
│   │
│   └── Exceptions/                            # Domain Exceptions
│       ├── DomainException.cs
│       ├── ContratoInvalidoException.cs
│       └── PropostaNaoAprovadaException.cs
│
├── ContratacaoService.Application/            # Camada de Aplicação (Casos de Uso)
│   ├── UseCases/                              # Use Cases / Application Services
│   │   ├── CriarContrato/
│   │   │   ├── CriarContratoCommand.cs       # Command (CQRS)
│   │   │   ├── CriarContratoHandler.cs       # Command Handler
│   │   │   └── CriarContratoValidator.cs     # FluentValidation
│   │   ├── ObterContrato/
│   │   │   ├── ObterContratoQuery.cs         # Query (CQRS)
│   │   │   └── ObterContratoHandler.cs       # Query Handler
│   │   ├── ListarContratos/
│   │   │   ├── ListarContratosQuery.cs
│   │   │   └── ListarContratosHandler.cs
│   │   └── VerificarProposta/
│   │       ├── VerificarPropostaQuery.cs
│   │       └── VerificarPropostaHandler.cs
│   │
│   ├── DTOs/                                  # Data Transfer Objects
│   │   ├── ContratoDto.cs
│   │   ├── CriarContratoDto.cs
│   │   └── PropostaDto.cs
│   │
│   ├── Mappings/                              # AutoMapper Profiles
│   │   └── ContratoMappingProfile.cs
│   │
│   ├── Interfaces/                            # Application Ports
│   │   ├── IContratoApplicationService.cs
│   │   └── IPropostaServiceClient.cs
│   │
│   ├── Behaviors/                             # MediatR Pipeline Behaviors
│   │   ├── ValidationBehavior.cs             # Comportamento de Validação
│   │   ├── LoggingBehavior.cs                # Comportamento de Log
│   │   └── TransactionBehavior.cs            # Comportamento Transacional
│   │
│   ├── Services/                              # Application Services
│   │   └── ContratoApplicationService.cs
│   │
│   └── Saga/                                  # Saga Pattern
│       ├── ContratacaoSaga.cs                # Saga Orchestrator
│       ├── SagaSteps/
│       │   ├── VerificarPropostaStep.cs
│       │   ├── CriarContratoStep.cs
│       │   └── NotificarContratoStep.cs
│       └── ISagaStep.cs
│
├── ContratacaoService.Infrastructure/         # ADAPTADORES - Camada de Infraestrutura
│   ├── Persistence/                           # Adapter para Persistência
│   │   ├── Context/
│   │   │   └── ContratacaoDbContext.cs       # EF Core DbContext
│   │   ├── Configurations/                    # EF Core Configurations
│   │   │   └── ContratoConfiguration.cs
│   │   ├── Repositories/                      # Implementação dos Repositories
│   │   │   ├── ContratoRepository.cs
│   │   │   └── UnitOfWork.cs
│   │   └── Migrations/                        # EF Core Migrations
│   │
│   ├── ExternalServices/                      # Adapter para Serviços Externos
│   │   ├── Messaging/                         # Message Broker (RabbitMQ)
│   │   │   ├── IMessagePublisher.cs
│   │   │   ├── IMessageConsumer.cs
│   │   │   ├── RabbitMqPublisher.cs
│   │   │   ├── RabbitMqConsumer.cs
│   │   │   └── Consumers/
│   │   │       ├── PropostaAprovadaConsumer.cs
│   │   │       └── PropostaRejeitadaConsumer.cs
│   │   └── HttpClients/                       # HTTP Clients
│   │       ├── IPropostaServiceClient.cs
│   │       └── PropostaServiceClient.cs
│   │
│   ├── CrossCutting/                          # Concerns Transversais
│   │   ├── Logging/
│   │   │   └── SerilogConfiguration.cs
│   │   ├── Caching/
│   │   │   └── RedisCacheService.cs
│   │   └── Security/
│   │       └── JwtConfiguration.cs
│   │
│   └── DependencyInjection/                   # IoC Container Configuration
│       └── InfrastructureModule.cs
│
├── ContratacaoService.Api/                    # ADAPTADOR - Camada de Apresentação (REST API)
│   ├── Controllers/                           # REST Controllers
│   │   └── ContratosController.cs
│   │
│   ├── Middlewares/                           # Custom Middlewares
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── LoggingMiddleware.cs
│   │   └── AuthenticationMiddleware.cs
│   │
│   ├── Filters/                               # Action Filters
│   │   ├── ValidateModelAttribute.cs
│   │   └── ApiExceptionFilterAttribute.cs
│   │
│   ├── ViewModels/                            # Request/Response Models
│   │   ├── Request/
│   │   │   └── CriarContratoRequest.cs
│   │   └── Response/
│   │       ├── ContratoResponse.cs
│   │       └── ApiErrorResponse.cs
│   │
│   ├── Configuration/                         # API Configuration
│   │   ├── SwaggerConfiguration.cs
│   │   ├── CorsConfiguration.cs
│   │   └── ApiVersioningConfiguration.cs
│   │
│   ├── BackgroundServices/                    # Background Workers
│   │   └── RabbitMqListenerService.cs
│   │
│   ├── Program.cs                             # Application Entry Point
│   └── appsettings.json                       # Configuration Files
│
└── ContratacaoService.Tests/                  # Camada de Testes
    ├── Unit/                                  # Testes Unitários
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   │   └── ContratoTests.cs
    │   │   ├── ValueObjects/
    │   │   │   └── PropostaIdTests.cs
    │   │   └── Services/
    │   │       └── ContratoValidationServiceTests.cs
    │   ├── Application/
    │   │   ├── UseCases/
    │   │   │   └── CriarContratoHandlerTests.cs
    │   │   └── Saga/
    │   │       └── ContratacaoSagaTests.cs
    │   └── Infrastructure/
    │       └── HttpClients/
    │           └── PropostaServiceClientTests.cs
    │
    ├── Integration/                           # Testes de Integração
    │   ├── Api/
    │   │   └── ContratosControllerTests.cs
    │   ├── Infrastructure/
    │   │   ├── ContratoRepositoryTests.cs
    │   │   └── RabbitMqIntegrationTests.cs
    │   └── Saga/
    │       └── ContratacaoSagaIntegrationTests.cs
    │
    ├── Fixtures/                              # Test Fixtures
    │   ├── ContratoFixture.cs
    │   └── PropostaFixture.cs
    │
    └── Builders/                              # Test Builders (Builder Pattern)
        ├── ContratoBuilder.cs
        └── PropostaBuilder.cs
```

## Princípios e Padrões Aplicados

### Arquitetura Hexagonal (Ports & Adapters)
- **Núcleo (Domain)**: Regras de negócio puras, independente de frameworks
- **Ports**: Interfaces que definem contratos (IRepository, IMessagePublisher)
- **Adapters**: Implementações concretas (PropostaRepository, RabbitMqPublisher)
- **Isolamento**: Domínio não conhece infraestrutura ou apresentação

### Domain-Driven Design (DDD)
- **Entities**: Objetos com identidade única (Proposta, Contrato)
- **Value Objects**: Objetos sem identidade, imutáveis (Cpf, Dinheiro)
- **Aggregates**: Cluster de entidades tratadas como unidade (Proposta é Aggregate Root)
- **Domain Events**: Eventos de domínio para comunicação entre contextos
- **Repositories**: Abstração para persistência focada no domínio
- **Domain Services**: Lógica de negócio que não pertence a uma entidade específica
- **Specifications**: Padrão para encapsular regras de negócio reutilizáveis

### SOLID
- **S** - Single Responsibility: Cada classe tem uma única responsabilidade
- **O** - Open/Closed: Extensível via interfaces, fechado para modificação
- **L** - Liskov Substitution: Implementações podem substituir abstrações
- **I** - Interface Segregation: Interfaces específicas (IPropostaRepository vs IUnitOfWork)
- **D** - Dependency Inversion: Dependência de abstrações, não implementações

### Design Patterns
- **Repository Pattern**: Abstração da camada de dados
- **Unit of Work**: Gerenciamento de transações
- **CQRS**: Separação de Commands e Queries
- **Mediator (MediatR)**: Desacoplamento de handlers
- **Builder Pattern**: Construção de objetos de teste
- **Specification Pattern**: Regras de negócio reutilizáveis
- **Strategy Pattern**: Comportamentos alternativos (validações)
- **Factory Pattern**: Criação de objetos complexos
- **Pipeline Behavior**: Cross-cutting concerns (validação, logging, transação)
- **Saga Pattern**: Orquestração de transações distribuídas entre microserviços

### Clean Code
- **Nomenclatura**: Nomes descritivos e significativos
- **Coesão**: Classes pequenas e focadas
- **Baixo Acoplamento**: Dependência de abstrações
- **Testabilidade**: Código fácil de testar
- **Separação de Responsabilidades**: Cada camada tem seu papel bem definido

## Comunicação entre Microserviços

### Padrão Saga para Contratação
O processo de contratação segue o padrão Saga para garantir consistência entre os serviços:

1. **ContratacaoService** recebe solicitação de contratação
2. **VerificarPropostaStep**: Consulta PropostaService (via RabbitMQ) para verificar status
3. **CriarContratoStep**: Se aprovada, cria o contrato no banco de dados
4. **NotificarContratoStep**: Publica evento de contrato criado

### Fluxo de Mensageria (RabbitMQ)
- **PropostaService** publica eventos: `PropostaAprovadaEvent`, `PropostaRejeitadaEvent`
- **ContratacaoService** consome eventos via `PropostaAprovadaConsumer`
- **ContratacaoService** consulta status via mensagem síncrona (Request/Reply pattern)
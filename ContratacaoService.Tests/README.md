# Testes Unitários - ContratacaoService

## Resumo

Este documento descreve todos os testes unitários e de integração implementados para o projeto ContratacaoService.

## Estrutura de Testes

### ?? Builders (Test Helpers)
- **ContratoBuilder**: Builder pattern para criar instâncias de Contrato para testes
- **PropostaBuilder**: Builder pattern para criar instâncias de Proposta para testes

### ?? Domain Layer Tests

#### Entities
1. **ContratoTests** (12 testes)
   - Criar contrato com dados válidos
   - Cancelar contrato
   - Suspender contrato
   - Reativar contrato
   - Expirar contrato
   - Validações de regras de negócio

#### Value Objects
1. **PropostaIdTests** (13 testes)
   - Criação com Guid válido
   - Validações (Guid Empty)
   - Conversões implícitas
   - Igualdade
   - Operadores

#### Services
1. **ContratoValidationServiceTests** (4 testes)
   - Validação de contrato válido
   - Validação de contrato nulo
   - Verificação de proposta disponível para contratação

#### Specifications
1. **ContratoAtivoSpecificationTests** (6 testes)
   - IsSatisfiedBy com diferentes status
   - ToExpression válida
   - Compilação e execução de expressão

#### Events
1. **ContratoEventTests** (3 testes)
   - Criação de eventos (ContratoCriado, ContratoAprovado, ContratoRejeitado)
   - Validação de dados dos eventos

#### Exceptions
1. **DomainExceptionTests** (3 testes)
   - Criação com mensagem
   - Lançamento de exceção

2. **ContratoInvalidoExceptionTests** (3 testes)
   - Criação com mensagem
   - Herança de DomainException
   - Lançamento de exceção

3. **PropostaNaoAprovadaExceptionTests** (3 testes)
   - Criação com mensagem
   - Herança de DomainException
   - Lançamento de exceção

### ?? Application Layer Tests

#### Use Cases
1. **ObterContratoHandlerTests** (2 testes)
   - Obtenção de contrato existente
   - Contrato inexistente retorna null

2. **ListarContratosHandlerTests** (2 testes)
   - Listagem de contratos existentes
   - Lista vazia quando não há contratos

#### Validators
1. **CriarContratoValidatorTests** (2 testes)
   - Validação de PropostaId válido
   - Validação de PropostaId vazio

#### Behaviors
1. **ValidationBehaviorTests** (4 testes)
   - Comportamento sem validadores
   - Validador válido chama próximo handler
   - Validador inválido lança exceção
   - Múltiplos validadores

#### Mappings
1. **ContratoMappingProfileTests** (4 testes)
   - Mapeamento Contrato para ContratoDto
   - Mapeamento de contrato cancelado
   - Mapeamento de lista
   - Configuração válida do AutoMapper

### ?? Infrastructure Layer Tests (Integration)

#### Repositories
1. **ContratoRepositoryTests** (8 testes)
   - Adicionar contrato
   - Obter por ID (existente e inexistente)
   - Listar todos (com e sem contratos)
   - Atualizar contrato
   - Verificar existência de contrato para proposta

2. **UnitOfWorkTests** (4 testes)
   - Commit com alterações pendentes
   - Commit sem alterações
   - Commit com múltiplas operações
   - Rollback

## Cobertura de Testes

### Estatísticas
- **Total de Testes**: 73 testes unitários e de integração
- **Camadas Cobertas**:
  - ? Domain (Entities, Value Objects, Services, Specifications, Events, Exceptions)
  - ? Application (Use Cases, Validators, Behaviors, Mappings)
  - ? Infrastructure (Repositories, UnitOfWork)

### Padrões Utilizados
- **AAA Pattern**: Arrange, Act, Assert
- **Builder Pattern**: Para construção de objetos de teste
- **Mocking**: Usando Moq para dependências
- **InMemory Database**: Para testes de integração
- **FluentAssertions**: Para asserções mais legíveis
- **Theory/InlineData**: Para testes parametrizados (onde aplicável)

## Ferramentas e Frameworks

- **xUnit**: Framework de testes
- **FluentAssertions**: Biblioteca de asserções fluentes
- **Moq**: Framework de mocking
- **Microsoft.EntityFrameworkCore.InMemory**: Banco em memória para testes
- **AutoMapper**: Testes de mapeamento
- **FluentValidation**: Testes de validação

## Como Executar os Testes

### Via Visual Studio
1. Abra o Test Explorer (Test > Test Explorer)
2. Click em "Run All" para executar todos os testes

### Via CLI
```bash
dotnet test ContratacaoService.Tests/ContratacaoService.Tests.csproj
```

### Com Cobertura de Código
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Boas Práticas Implementadas

1. **Isolamento**: Cada teste é independente
2. **Nomenclatura Clara**: Nome dos testes segue padrão `Metodo_Cenario_ResultadoEsperado`
3. **Builders**: Facilitam criação de objetos complexos
4. **Arrange-Act-Assert**: Estrutura clara em todos os testes
5. **InMemory Database**: Testes de integração sem dependência de banco real
6. **Mocking**: Isolamento de dependências externas
7. **FluentAssertions**: Asserções mais legíveis e com melhor feedback

## Comparação com PropostaService.Tests

Este projeto de testes foi implementado seguindo o mesmo padrão estabelecido em PropostaService.Tests:

- ? Mesma estrutura de pastas
- ? Mesmos frameworks e bibliotecas
- ? Mesmos padrões de nomenclatura
- ? Mesma abordagem de Builders
- ? Mesmas práticas de isolamento e mocking
- ? Mesma organização por camadas

## Melhorias Futuras

- [ ] Adicionar testes para os Saga Steps
- [ ] Adicionar testes para o ContratacaoSaga
- [ ] Adicionar testes de performance
- [ ] Adicionar testes end-to-end da API
- [ ] Configurar relatórios de cobertura automáticos
- [ ] Adicionar testes de mutação

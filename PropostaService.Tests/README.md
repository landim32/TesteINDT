# Testes Unitários - PropostaService

## Resumo

Este documento descreve todos os testes unitários e de integração implementados para o projeto PropostaService.

## Estrutura de Testes

### ?? Builders (Test Helpers)
- **PropostaBuilder**: Builder pattern para criar instâncias de Proposta para testes
- **ClienteBuilder**: Builder pattern para criar instâncias de Cliente para testes
- **SeguroBuilder**: Builder pattern para criar instâncias de Seguro para testes

### ?? Domain Layer Tests

#### Entities
1. **PropostaTests** (12 testes)
   - Criar proposta com dados válidos
   - Aprovar proposta
   - Rejeitar proposta
   - Alterar status
   - Atualizar cliente
   - Atualizar seguro
   - Validações de regras de negócio

2. **PropostaDomainEventsTests** (9 testes)
   - Eventos de criação, aprovação e rejeição
   - Acumulação de eventos
   - Limpeza de eventos
   - ReadOnly collection

3. **ClienteTests** (8 testes)
   - Criação com dados válidos
   - Validações de nome (vazio, muito curto, muito longo)
   - Trimming de espaços
   - Atualização de nome

4. **SeguroTests** (11 testes)
   - Criação com dados válidos
   - Validações de valores (zero, negativo)
   - Validação prêmio vs cobertura
   - Atualização de valores

#### Value Objects
1. **CpfTests** (7 testes)
   - Criação com CPF válido
   - Validações (vazio, inválido)
   - Formatação
   - Igualdade

2. **DinheiroTests** (16 testes)
   - Criação com valores válidos
   - Operações (somar, subtrair)
   - Comparações (==, !=, >, <, >=, <=)
   - Validações
   - ToString e GetHashCode

3. **TipoSeguroTests** (13 testes)
   - Criação com valores válidos
   - Validações (vazio, muito curto, muito longo)
   - Tipos predefinidos (Vida, Auto, Residencial, Saúde)
   - Igualdade case-insensitive

#### Services
1. **PropostaValidationServiceTests** (4 testes)
   - Validação de proposta válida
   - Validação de proposta nula
   - Obtenção de erros de validação

#### Specifications
1. **PropostaAprovadaSpecificationTests** (6 testes)
   - IsSatisfiedBy com diferentes status
   - ToExpression válida
   - Compilação e execução de expressão

#### Events
1. **PropostaEventTests** (3 testes)
   - Criação de eventos (PropostaCriada, PropostaAprovada, PropostaRejeitada)
   - Validação de dados dos eventos

#### Exceptions
1. **DomainExceptionTests** (3 testes)
   - Criação com mensagem
   - Lançamento de exceção

2. **PropostaInvalidaExceptionTests** (3 testes)
   - Criação com mensagem
   - Herança de DomainException
   - Lançamento de exceção

### ?? Application Layer Tests

#### Use Cases
1. **CriarPropostaHandlerTests** (2 testes)
   - Criação de proposta com dados válidos
   - Retorno de ID da proposta

2. **AtualizarPropostaHandlerTests** (5 testes)
   - Atualização com proposta existente
   - Proposta inexistente
   - Atualização apenas de nome
   - Atualização apenas de valores
   - Proposta aprovada não pode ser atualizada

3. **AlterarStatusPropostaHandlerTests** (4 testes)
   - Alteração de status com proposta existente
   - Proposta inexistente
   - Alteração para rejeitada
   - Alteração de status de proposta aprovada

4. **ObterPropostaHandlerTests** (2 testes)
   - Obtenção de proposta existente
   - Proposta inexistente retorna null

5. **ListarPropostasHandlerTests** (2 testes)
   - Listagem de propostas existentes
   - Lista vazia quando não há propostas

#### Validators
1. **CriarPropostaValidatorTests** (14 testes)
   - Validação de todos os campos
   - Validações de tamanho e formato
   - CPF formatado e não formatado
   - Validações de valores

2. **AtualizarPropostaValidatorTests** (11 testes)
   - Validação de ID obrigatório
   - Validações condicionais de campos opcionais
   - Validações de tamanho
   - Validações de valores quando presentes

3. **AlterarStatusPropostaValidatorTests** (4 testes)
   - Validação de ID obrigatório
   - Validação de status válido
   - Status inválido

#### Behaviors
1. **ValidationBehaviorTests** (4 testes)
   - Comportamento sem validadores
   - Validador válido chama próximo handler
   - Validador inválido lança exceção
   - Múltiplos validadores

#### Mappings
1. **PropostaMappingProfileTests** (4 testes)
   - Mapeamento Proposta para PropostaDto
   - Mapeamento de proposta aprovada
   - Mapeamento de lista
   - Configuração válida do AutoMapper

### ?? Infrastructure Layer Tests (Integration)

#### Repositories
1. **PropostaRepositoryTests** (8 testes)
   - Adicionar proposta
   - Obter por ID (existente e inexistente)
   - Obter todas (com e sem propostas)
   - Obter por status
   - Atualizar proposta
   - Atualizar status

2. **UnitOfWorkTests** (4 testes)
   - Commit com alterações pendentes
   - Commit sem alterações
   - Commit com múltiplas operações
   - Rollback

## Cobertura de Testes

### Estatísticas
- **Total de Testes**: 140+ testes unitários e de integração
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
- **Theory/InlineData**: Para testes parametrizados

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
dotnet test PropostaService.Tests/PropostaService.Tests.csproj
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
5. **Testes Parametrizados**: Usando Theory para múltiplos cenários
6. **InMemory Database**: Testes de integração sem dependência de banco real
7. **Mocking**: Isolamento de dependências externas
8. **FluentAssertions**: Asserções mais legíveis e com melhor feedback

## Melhorias Futuras

- [ ] Adicionar testes de performance
- [ ] Adicionar testes de carga
- [ ] Adicionar testes end-to-end da API
- [ ] Configurar relatórios de cobertura automáticos
- [ ] Adicionar testes de mutação

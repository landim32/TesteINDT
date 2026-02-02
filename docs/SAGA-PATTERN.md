# Padrão Saga - Implementação no ContratacaoService

## ?? Visão Geral

O **Saga Pattern** é um padrão de design para gerenciar transações distribuídas em arquiteturas de microserviços. Ele garante a consistência de dados através de uma sequência de transações locais, onde cada transação atualiza dados em um único serviço e publica eventos para disparar a próxima transação na sequência.

## ??? Arquitetura da Saga de Contratação

### Fluxo Principal

```
???????????????????????????????????????????????????????????????????
?                    ContratacaoSaga                              ?
?                                                                 ?
?  1. VerificarPropostaStep                                      ?
?     ??> Consulta PropostaService via HTTP                     ?
?     ??> Valida se proposta está APROVADA                      ?
?                                                                 ?
?  2. CriarContratoStep                                          ?
?     ??> Cria contrato no banco de dados                       ?
?     ??> Persiste via UnitOfWork                               ?
?                                                                 ?
?  3. NotificarContratoStep                                      ?
?     ??> Publica evento ContratoCriadoEvent                    ?
?     ??> Via MediatR para processamento assíncrono             ?
?                                                                 ?
?  ? SUCESSO: Retorna contrato criado                           ?
?                                                                 ?
?  ? ERRO: Executa compensação                                  ?
?     ??> CriarContratoStep.CompensateAsync()                   ?
?     ??> Cancela o contrato criado                             ?
???????????????????????????????????????????????????????????????????
```

## ?? Componentes da Implementação

### 1. ISagaStep<TInput, TOutput>

Interface genérica que define o contrato para cada etapa da Saga:

```csharp
public interface ISagaStep<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    Task CompensateAsync(TInput input, CancellationToken cancellationToken = default);
}
```

**Responsabilidades:**
- `ExecuteAsync`: Executa a lógica de negócio da etapa
- `CompensateAsync`: Desfaz as mudanças em caso de falha (transação compensatória)

### 2. VerificarPropostaStep

**Objetivo:** Verificar se a proposta existe e está aprovada.

```csharp
public class VerificarPropostaStep : ISagaStep<Guid, PropostaDto>
{
    public async Task<PropostaDto> ExecuteAsync(Guid propostaId, ...)
    {
        // 1. Consulta PropostaService via HTTP
        var proposta = await _propostaServiceClient.ObterPropostaAsync(propostaId);
        
        // 2. Valida se proposta existe
        if (proposta == null)
            throw new ContratoInvalidoException(...);
        
        // 3. Valida se proposta está aprovada
        if (proposta.Status != StatusProposta.Aprovada)
            throw new PropostaNaoAprovadaException(...);
        
        return proposta;
    }
    
    public Task CompensateAsync(Guid propostaId, ...)
    {
        // Não há compensação necessária
        return Task.CompletedTask;
    }
}
```

**Características:**
- ? Comunicação síncrona (HTTP) com PropostaService
- ? Validação de estado da proposta
- ? Sem compensação (operação read-only)

### 3. CriarContratoStep

**Objetivo:** Criar o contrato no banco de dados.

```csharp
public class CriarContratoStep : ISagaStep<PropostaDto, Contrato>
{
    public async Task<Contrato> ExecuteAsync(PropostaDto proposta, ...)
    {
        // 1. Cria entidade de domínio
        var contrato = new Contrato(new PropostaId(proposta.Id));
        
        // 2. Adiciona ao repositório
        await _contratoRepository.AdicionarAsync(contrato);
        
        // 3. Persiste no banco
        await _unitOfWork.CommitAsync();
        
        return contrato;
    }
    
    public async Task CompensateAsync(PropostaDto proposta, ...)
    {
        // Compensação: Cancela o contrato criado
        var contrato = await _contratoRepository.ObterPorPropostaIdAsync(proposta.Id);
        if (contrato != null)
        {
            contrato.Cancelar(); // Método do domínio
            await _contratoRepository.AtualizarAsync(contrato);
            await _unitOfWork.CommitAsync();
        }
    }
}
```

**Características:**
- ? Persistência no banco de dados
- ? Usa agregado do domínio (Contrato)
- ? **Compensação implementada** - Cancela contrato em caso de falha

### 4. NotificarContratoStep

**Objetivo:** Publicar evento de contrato criado.

```csharp
public class NotificarContratoStep : ISagaStep<Contrato, bool>
{
    public async Task<bool> ExecuteAsync(Contrato contrato, ...)
    {
        // Publica evento de domínio via MediatR
        var evento = new ContratoCriadoEvent(
            contrato.Id,
            contrato.PropostaId.Value,
            contrato.DataContratacao
        );
        
        await _mediator.Publish(evento);
        return true;
    }
    
    public Task CompensateAsync(Contrato contrato, ...)
    {
        // Não há compensação necessária
        return Task.CompletedTask;
    }
}
```

**Características:**
- ? Publica evento de domínio
- ? Comunicação assíncrona via MediatR
- ? Sem compensação (evento já foi publicado)

### 5. ContratacaoSaga (Orchestrator)

**Objetivo:** Orquestrar a execução de todas as etapas.

```csharp
public class ContratacaoSaga
{
    public async Task<Contrato> ExecutarAsync(Guid propostaId, ...)
    {
        PropostaDto? proposta = null;
        Contrato? contrato = null;
        
        try
        {
            // Etapa 1: Verificar Proposta
            proposta = await _verificarPropostaStep.ExecuteAsync(propostaId);
            
            // Etapa 2: Criar Contrato
            contrato = await _criarContratoStep.ExecuteAsync(proposta);
            
            // Etapa 3: Notificar
            await _notificarContratoStep.ExecuteAsync(contrato);
            
            return contrato;
        }
        catch (Exception ex)
        {
            // Compensação: Desfaz etapas já executadas
            if (contrato != null && proposta != null)
            {
                await _criarContratoStep.CompensateAsync(proposta);
            }
            throw;
        }
    }
}
```

## ?? Fluxo de Compensação

### Cenário de Falha

```
Etapa 1: VerificarPropostaStep ? SUCESSO
         ??> Proposta encontrada e aprovada

Etapa 2: CriarContratoStep ? SUCESSO
         ??> Contrato criado e persistido

Etapa 3: NotificarContratoStep ? FALHA
         ??> Erro ao publicar evento

Compensação:
         ??> CriarContratoStep.CompensateAsync()
         ??> Contrato.Cancelar()
         ??> Status alterado para CANCELADO
         ??> Mudança persistida no banco
```

## ?? Vantagens da Implementação

### 1. **Consistência Eventual**
- ? Garante consistência entre microserviços
- ? Cada etapa é uma transação local bem-sucedida
- ? Compensação automática em caso de falha

### 2. **Separação de Responsabilidades**
- ? Cada step tem uma responsabilidade única
- ? Fácil adicionar/remover etapas
- ? Testável individualmente

### 3. **Rastreabilidade**
- ? Logs detalhados de cada etapa
- ? Fácil identificar onde a saga falhou
- ? Auditoria completa do fluxo

### 4. **Resiliência**
- ? Rollback automático via compensação
- ? Sistema mantém consistência mesmo com falhas
- ? Não deixa dados órfãos no banco

## ?? Uso no Handler

### CriarContratoHandler (Simplificado)

**Antes (Sem Saga):**
```csharp
public async Task<ContratoDto> Handle(...)
{
    // 1. Verificar proposta
    var proposta = await _propostaServiceClient.ObterPropostaAsync(...);
    if (proposta == null) throw ...;
    if (proposta.Status != Aprovada) throw ...;
    
    // 2. Validar duplicação
    var podeContratar = await _contratoValidationService...;
    if (!podeContratar) throw ...;
    
    // 3. Criar contrato
    var contrato = new Contrato(...);
    await _contratoRepository.AdicionarAsync(...);
    await _unitOfWork.CommitAsync();
    
    // 4. Publicar evento
    await _mediator.Publish(...);
    
    // ? Sem compensação automática
    // ? Lógica acoplada ao handler
    // ? Difícil de testar
}
```

**Depois (Com Saga):**
```csharp
public async Task<ContratoDto> Handle(...)
{
    var contrato = await _contratacaoSaga.ExecutarAsync(request.PropostaId);
    return _mapper.Map<ContratoDto>(contrato);
    
    // ? Compensação automática
    // ? Lógica isolada na saga
    // ? Fácil de testar
}
```

## ?? Testando a Saga

### Teste de Sucesso

```csharp
[Fact]
public async Task ExecutarAsync_PropostaAprovada_DeveCriarContrato()
{
    // Arrange
    var propostaId = Guid.NewGuid();
    var proposta = new PropostaDto { Id = propostaId, Status = StatusProposta.Aprovada };
    
    _verificarPropostaStep
        .Setup(x => x.ExecuteAsync(propostaId, default))
        .ReturnsAsync(proposta);
    
    // Act
    var resultado = await _saga.ExecutarAsync(propostaId);
    
    // Assert
    resultado.Should().NotBeNull();
    _criarContratoStep.Verify(x => x.ExecuteAsync(proposta, default), Times.Once);
    _notificarContratoStep.Verify(x => x.ExecuteAsync(It.IsAny<Contrato>(), default), Times.Once);
}
```

### Teste de Compensação

```csharp
[Fact]
public async Task ExecutarAsync_ErroAoNotificar_DeveCompensarContrato()
{
    // Arrange
    var proposta = new PropostaDto { ... };
    var contrato = new Contrato(...);
    
    _notificarContratoStep
        .Setup(x => x.ExecuteAsync(contrato, default))
        .ThrowsAsync(new Exception("Erro ao publicar evento"));
    
    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => _saga.ExecutarAsync(propostaId));
    
    // Verify: Compensação foi chamada
    _criarContratoStep.Verify(x => x.CompensateAsync(proposta, default), Times.Once);
}
```

## ?? Executando a Saga

### Via API

```bash
# 1. Criar e aprovar proposta
curl -X POST http://localhost:5000/api/propostas \
  -d '{"nomeCliente":"João","cpf":"12345678909",...}'

curl -X PATCH http://localhost:5000/api/propostas/{id}/status \
  -d '{"status": 2}'

# 2. Criar contrato (executa a Saga)
curl -X POST http://localhost:5001/api/contratos \
  -d '{"propostaId":"{id}"}'
```

### Logs da Execução

```
[INFO] Iniciando saga de contratação para proposta {PropostaId}
[INFO] Verificando proposta {PropostaId}
[INFO] Proposta {PropostaId} verificada com sucesso
[INFO] Criando contrato para proposta {PropostaId}
[INFO] Contrato {ContratoId} criado com sucesso
[INFO] Notificando criação do contrato {ContratoId}
[INFO] Saga de contratação concluída com sucesso
```

### Logs de Compensação

```
[ERROR] Erro na saga de contratação para proposta {PropostaId}
[WARN] Compensando criação de contrato para proposta {PropostaId}
[INFO] Contrato {ContratoId} cancelado com sucesso
```

## ?? Referências

- **Saga Pattern**: https://microservices.io/patterns/data/saga.html
- **Orchestration vs Choreography**: Esta implementação usa **Orchestration** (ContratacaoSaga como orquestrador)
- **Event-Driven Architecture**: Integração via eventos de domínio (MediatR)

## ?? Lições Aprendidas

1. **Saga não é transação ACID**: É consistência eventual
2. **Compensação deve ser idempotente**: Pode ser executada múltiplas vezes
3. **Ordem importa**: Compensação é executada na ordem inversa
4. **Logs são essenciais**: Para rastreabilidade e debugging
5. **Cada step é uma transação local**: Commit por step, não no final

## ? Checklist de Implementação

- [x] Interface `ISagaStep<TInput, TOutput>`
- [x] Step 1: `VerificarPropostaStep`
- [x] Step 2: `CriarContratoStep` com compensação
- [x] Step 3: `NotificarContratoStep`
- [x] Orchestrator: `ContratacaoSaga`
- [x] Integração no Handler
- [x] Logging em todas as etapas
- [x] Tratamento de exceções
- [x] Injeção de dependência configurada

?? **Padrão Saga implementado com sucesso!**

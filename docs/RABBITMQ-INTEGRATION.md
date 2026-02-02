# Integração com RabbitMQ - ContratoCriadoEvent

## ?? Problema Resolvido

O `ContratoCriadoEvent` estava sendo publicado apenas via MediatR (in-process), mas não estava sendo enviado para o RabbitMQ para comunicação entre microserviços.

## ?? Solução Implementada

### 1. Event Handler para RabbitMQ

Criado `ContratoCriadoEventHandler` que intercepta o evento do MediatR e o publica no RabbitMQ.

**Arquivo:** `ContratacaoService.Infrastructure/EventHandlers/ContratoCriadoEventHandler.cs`

```csharp
public class ContratoCriadoEventHandler : INotificationHandler<ContratoCriadoEvent>
{
    public async Task Handle(ContratoCriadoEvent notification, CancellationToken cancellationToken)
    {
        // 1. Lê configuração do exchange
        var exchange = _configuration["RabbitMQ:Exchanges:Contratos"] ?? "contratos.exchange";
        var routingKey = "contrato.criado";
        
        // 2. Publica no RabbitMQ
        await _messagePublisher.PublishAsync(exchange, routingKey, notification, cancellationToken);
    }
}
```

### 2. Inicializador do RabbitMQ

Criado `RabbitMqInitializer` para declarar exchanges e queues na inicialização da aplicação.

**Arquivo:** `ContratacaoService.Infrastructure/ExternalServices/Messaging/RabbitMqInitializer.cs`

```csharp
public class RabbitMqInitializer
{
    public void Initialize()
    {
        // 1. Declarar exchange
        channel.ExchangeDeclare(
            exchange: "contratos.exchange",
            type: ExchangeType.Topic,
            durable: true);
        
        // 2. Declarar queue
        channel.QueueDeclare(
            queue: "contrato.criado.queue",
            durable: true);
        
        // 3. Bind queue ao exchange
        channel.QueueBind(
            queue: "contrato.criado.queue",
            exchange: "contratos.exchange",
            routingKey: "contrato.criado");
    }
}
```

### 3. Registro no MediatR

Atualizado `InfrastructureModule` para registrar handlers do assembly Infrastructure:

```csharp
services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ContratoApplicationService).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ContratoCriadoEventHandler).Assembly); // ? NOVO
});
```

### 4. Inicialização no Program.cs

```csharp
using (var scope = app.Services.CreateScope())
{
    // Migrations
    var dbContext = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
    await dbContext.Database.MigrateAsync();
    
    // Inicializar RabbitMQ ? NOVO
    var rabbitMqInitializer = scope.ServiceProvider.GetRequiredService<RabbitMqInitializer>();
    rabbitMqInitializer.Initialize();
}
```

## ?? Fluxo Completo

```
????????????????????????????????????????????????????????????????
?                    ContratacaoSaga                           ?
?                                                              ?
?  1. VerificarPropostaStep                                   ?
?  2. CriarContratoStep                                       ?
?  3. NotificarContratoStep                                   ?
?     ??> _mediator.Publish(ContratoCriadoEvent)             ?
????????????????????????????????????????????????????????????????
                          ?
????????????????????????????????????????????????????????????????
?                 MediatR (In-Process)                         ?
?                                                              ?
?  Distribui para todos os handlers registrados:              ?
?  - ContratoCriadoEventHandler (Infrastructure)              ?
????????????????????????????????????????????????????????????????
                          ?
????????????????????????????????????????????????????????????????
?            ContratoCriadoEventHandler                        ?
?                                                              ?
?  await _messagePublisher.PublishAsync(                      ?
?      exchange: "contratos.exchange",                        ?
?      routingKey: "contrato.criado",                         ?
?      message: ContratoCriadoEvent                           ?
?  )                                                           ?
????????????????????????????????????????????????????????????????
                          ?
????????????????????????????????????????????????????????????????
?                   RabbitMqPublisher                          ?
?                                                              ?
?  1. Serializa evento para JSON                              ?
?  2. Cria propriedades (Persistent, ContentType)            ?
?  3. Publica no exchange                                     ?
????????????????????????????????????????????????????????????????
                          ?
????????????????????????????????????????????????????????????????
?                     RabbitMQ                                 ?
?                                                              ?
?  Exchange: contratos.exchange (Topic)                       ?
?      ?                                                       ?
?  Routing Key: contrato.criado                               ?
?      ?                                                       ?
?  Queue: contrato.criado.queue                               ?
????????????????????????????????????????????????????????????????
                          ?
                Consumidores (Outros Microserviços)
```

## ?? Estrutura do RabbitMQ

### Exchange
```
Nome: contratos.exchange
Tipo: Topic
Durável: true
Auto-delete: false
```

### Queue
```
Nome: contrato.criado.queue
Durável: true
Exclusiva: false
Auto-delete: false
```

### Binding
```
Queue: contrato.criado.queue
Exchange: contratos.exchange
Routing Key: contrato.criado
```

## ?? Mensagem Publicada

```json
{
  "contratoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "propostaId": "82477b50-b5dc-482c-b64a-3962a12303aa",
  "dataContratacao": "2026-02-02T10:30:00Z"
}
```

**Propriedades da Mensagem:**
- Content-Type: `application/json`
- Persistent: `true`
- Routing Key: `contrato.criado`

## ?? Testando a Integração

### 1. Verificar RabbitMQ Management

Acesse: http://localhost:15672 (guest/guest)

**Verificações:**
- ? Exchange `contratos.exchange` existe
- ? Queue `contrato.criado.queue` existe
- ? Binding entre exchange e queue configurado
- ? Mensagens aparecem na queue após criar contrato

### 2. Criar Contrato

```sh
# 1. Criar proposta
curl -X POST http://localhost:5000/api/propostas \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCliente": "João Silva",
    "cpf": "12345678909",
    "tipoSeguro": "Auto",
    "valorCobertura": 50000,
    "valorPremio": 1200
  }'

# 2. Aprovar proposta
curl -X PATCH http://localhost:5000/api/propostas/{id}/status \
  -H "Content-Type: application/json" \
  -d '{"status": 2}'

# 3. Criar contrato (dispara evento)
curl -X POST http://localhost:5001/api/contratos \
  -H "Content-Type: application/json" \
  -d '{"propostaId":"{id}"}'
```

### 3. Verificar Logs

```
[INFO] Notificando criação do contrato {ContratoId}
[INFO] Notificação enviada para contrato {ContratoId}
[INFO] Publicando evento ContratoCriadoEvent para contrato {ContratoId} no RabbitMQ
[INFO] Mensagem publicada no exchange contratos.exchange com routing key contrato.criado
[INFO] Evento ContratoCriadoEvent publicado com sucesso para contrato {ContratoId}
```

### 4. Consumir Mensagem

No RabbitMQ Management:
1. Vá para "Queues"
2. Clique em `contrato.criado.queue`
3. Na aba "Get messages", clique em "Get Message(s)"
4. Verifique o JSON do evento

## ?? Benefícios da Implementação

### 1. **Separação de Responsabilidades**
- MediatR: Comunicação in-process
- RabbitMQ: Comunicação entre microserviços
- Event Handler: Ponte entre os dois

### 2. **Desacoplamento**
```csharp
// Saga não precisa conhecer RabbitMQ
await _mediator.Publish(evento);

// Handler faz a ponte
public class ContratoCriadoEventHandler : INotificationHandler<...>
{
    // Publica no RabbitMQ
}
```

### 3. **Testabilidade**
```csharp
// Pode testar Saga sem RabbitMQ
var mediator = new Mock<IMediator>();

// Pode testar Handler isoladamente
var publisher = new Mock<IMessagePublisher>();
```

### 4. **Rastreabilidade**
- Logs em cada etapa
- Fácil debugar problemas
- Auditoria completa

### 5. **Resiliência**
- Mensagens persistentes
- Queue durável
- Não perde mensagens se serviço cair

## ?? Configuração

### appsettings.json

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": "5672",
    "Username": "guest",
    "Password": "guest",
    "Exchanges": {
      "Contratos": "contratos.exchange"
    },
    "Queues": {
      "ContratoCriado": "contrato.criado.queue"
    }
  }
}
```

## ?? Padrões Utilizados

- ? **Event-Driven Architecture**: Comunicação baseada em eventos
- ? **Publisher-Subscriber**: RabbitMQ distribui eventos
- ? **Domain Events**: Eventos ricos do domínio
- ? **CQRS**: Separação de comandos e eventos
- ? **Mediator Pattern**: MediatR para in-process
- ? **Adapter Pattern**: RabbitMqPublisher adapta para RabbitMQ

## ?? Próximos Passos

### 1. Criar Consumer no PropostaService

```csharp
public class ContratoCriadoConsumer : INotificationHandler<ContratoCriadoEvent>
{
    public Task Handle(ContratoCriadoEvent notification, ...)
    {
        // Atualizar status da proposta
        // Enviar notificação ao cliente
        // etc.
    }
}
```

### 2. Adicionar Eventos Adicionais

- ContratoAprovadoEvent
- ContratoRejeitadoEvent
- ContratoCanceladoEvent

### 3. Dead Letter Queue

Configurar DLQ para mensagens que falharem:

```csharp
channel.QueueDeclare(
    queue: "contrato.criado.dlq",
    arguments: new Dictionary<string, object>
    {
        { "x-dead-letter-exchange", "dlx.exchange" }
    });
```

## ? Checklist de Implementação

- [x] `ContratoCriadoEventHandler` criado
- [x] `RabbitMqInitializer` criado
- [x] Handler registrado no MediatR
- [x] Initializer registrado no DI
- [x] Program.cs atualizado
- [x] Exchange declarado
- [x] Queue declarada
- [x] Binding configurado
- [x] Logs adicionados
- [x] Documentação criada

?? **Integração com RabbitMQ implementada com sucesso!**

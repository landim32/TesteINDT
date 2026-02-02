# ?? Guia RabbitMQ - Sistema de Propostas de Seguro

Este guia detalha como usar o RabbitMQ para mensageria assíncrona entre os microserviços.

## ?? O que é RabbitMQ?

RabbitMQ é um message broker (intermediário de mensagens) open-source que implementa o protocolo AMQP (Advanced Message Queuing Protocol). Ele permite comunicação assíncrona entre microserviços através de filas de mensagens.

## ?? Configuração

### Container Docker

O RabbitMQ está configurado no `docker-compose.yml`:

```yaml
rabbitmq:
  image: rabbitmq:3.13-management-alpine
  container_name: propostas-rabbitmq
  environment:
    RABBITMQ_DEFAULT_USER: guest
    RABBITMQ_DEFAULT_PASS: guest
    RABBITMQ_DEFAULT_VHOST: /
  ports:
    - "5672:5672"      # AMQP port
    - "15672:15672"    # Management UI
```

### Variáveis de Ambiente (.env)

```env
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
RABBITMQ_VHOST=/
RABBITMQ_PORT=5672
RABBITMQ_MANAGEMENT_PORT=15672
```

## ?? Management Console

### Acessar a Interface Web

1. Abra o navegador em: http://localhost:15672
2. Faça login com as credenciais:
   - **Username**: guest (ou conforme configurado no .env)
   - **Password**: guest (ou conforme configurado no .env)

### Funcionalidades da Console

- **Overview**: Visão geral do sistema
- **Connections**: Conexões ativas
- **Channels**: Canais abertos
- **Exchanges**: Exchanges configurados
- **Queues**: Filas e suas estatísticas
- **Admin**: Gerenciamento de usuários e permissões

## ?? Conceitos Principais

### 1. Producer (Produtor)
Aplicação que envia mensagens para o RabbitMQ.

### 2. Queue (Fila)
Buffer que armazena mensagens.

### 3. Consumer (Consumidor)
Aplicação que recebe e processa mensagens da fila.

### 4. Exchange
Recebe mensagens dos producers e as roteia para as filas.

**Tipos de Exchange:**
- **Direct**: Roteamento baseado em routing key exata
- **Topic**: Roteamento baseado em pattern matching
- **Fanout**: Broadcast para todas as filas
- **Headers**: Roteamento baseado em headers

### 5. Binding
Conexão entre Exchange e Queue.

### 6. Virtual Host (vhost)
Namespace lógico para separar ambientes.

## ?? Uso no Projeto

### Fluxo de Mensagens

```
PropostaService (Producer)
    ?
Exchange: "propostas.events"
    ?
Queue: "proposta.criada"
    ?
ContratacaoService (Consumer)
```

### Eventos do Domínio

1. **PropostaCriadaEvent**
   - Queue: `proposta.criada`
   - Consumer: ContratacaoService

2. **PropostaAprovadaEvent**
   - Queue: `proposta.aprovada`
   - Consumer: ContratacaoService

3. **PropostaRejeitadaEvent**
   - Queue: `proposta.rejeitada`
   - Consumer: NotificacaoService

## ?? Comandos Úteis

### Via Docker

```bash
# Status do RabbitMQ
docker exec -it propostas-rabbitmq rabbitmqctl status

# Listar filas
docker exec -it propostas-rabbitmq rabbitmqctl list_queues name messages consumers

# Listar exchanges
docker exec -it propostas-rabbitmq rabbitmqctl list_exchanges name type

# Listar bindings
docker exec -it propostas-rabbitmq rabbitmqctl list_bindings

# Listar usuários
docker exec -it propostas-rabbitmq rabbitmqctl list_users

# Listar vhosts
docker exec -it propostas-rabbitmq rabbitmqctl list_vhosts

# Limpar uma fila
docker exec -it propostas-rabbitmq rabbitmqctl purge_queue nome_da_fila

# Reiniciar RabbitMQ
docker-compose restart rabbitmq
```

### Via Management API

```bash
# Listar filas (via API REST)
curl -u guest:guest http://localhost:15672/api/queues

# Listar exchanges
curl -u guest:guest http://localhost:15672/api/exchanges

# Estatísticas de uma fila
curl -u guest:guest http://localhost:15672/api/queues/%2F/nome_da_fila
```

## ?? Monitoramento

### Health Check

```bash
# Verificar se RabbitMQ está rodando
docker exec -it propostas-rabbitmq rabbitmq-diagnostics ping

# Status detalhado
docker exec -it propostas-rabbitmq rabbitmq-diagnostics status

# Verificar nós do cluster
docker exec -it propostas-rabbitmq rabbitmqctl cluster_status
```

### Métricas Importantes

- **Message Rate**: Taxa de mensagens por segundo
- **Ready Messages**: Mensagens prontas para consumo
- **Unacked Messages**: Mensagens não confirmadas
- **Total Messages**: Total de mensagens na fila
- **Consumer Count**: Número de consumidores ativos

## ??? Configuração da Aplicação .NET

### Instalar Pacote NuGet

```bash
dotnet add package RabbitMQ.Client --version 6.8.1
```

### Connection String

```csharp
// appsettings.json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
  }
}
```

### Exemplo de Publisher

```csharp
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(string hostname, string username, string password)
    {
        var factory = new ConnectionFactory()
        {
            HostName = hostname,
            UserName = username,
            Password = password
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(string exchange, string routingKey, T message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body);
    }
}
```

### Exemplo de Consumer

```csharp
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumer(string hostname, string username, string password)
    {
        var factory = new ConnectionFactory()
        {
            HostName = hostname,
            UserName = username,
            Password = password
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(
            queue: "proposta.criada",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            // Processar mensagem
            ProcessMessage(message);
            
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        
        _channel.BasicConsume(
            queue: "proposta.criada",
            autoAck: false,
            consumer: consumer);
        
        return Task.CompletedTask;
    }
}
```

## ?? Segurança

### Boas Práticas

1. **Não use credenciais padrão em produção**
   ```bash
   # Criar novo usuário
   docker exec -it propostas-rabbitmq rabbitmqctl add_user myuser mypassword
   
   # Definir permissões
   docker exec -it propostas-rabbitmq rabbitmqctl set_permissions -p / myuser ".*" ".*" ".*"
   
   # Deletar usuário guest
   docker exec -it propostas-rabbitmq rabbitmqctl delete_user guest
   ```

2. **Use SSL/TLS em produção**

3. **Configure Virtual Hosts separados por ambiente**
   ```bash
   docker exec -it propostas-rabbitmq rabbitmqctl add_vhost production
   docker exec -it propostas-rabbitmq rabbitmqctl add_vhost development
   ```

4. **Implemente Dead Letter Queues**

5. **Configure TTL para mensagens**

## ?? Troubleshooting

### Container não inicia

```bash
# Ver logs
docker-compose logs rabbitmq

# Verificar portas
netstat -an | grep 5672
netstat -an | grep 15672
```

### Mensagens não são consumidas

```bash
# Verificar consumidores
docker exec -it propostas-rabbitmq rabbitmqctl list_queues name consumers

# Verificar se há mensagens na fila
docker exec -it propostas-rabbitmq rabbitmqctl list_queues name messages
```

### Resetar RabbitMQ

```bash
# ?? ATENÇÃO: Apaga todas as mensagens e configurações!
docker-compose down
docker volume rm testeindtt_rabbitmq_data
docker volume rm testeindtt_rabbitmq_logs
docker-compose up -d rabbitmq
```

## ?? Recursos Adicionais

- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
- [RabbitMQ .NET Client Guide](https://www.rabbitmq.com/dotnet-api-guide.html)
- [Management Plugin](https://www.rabbitmq.com/management.html)

## ?? Próximos Passos

1. Implementar IMessagePublisher no PropostaService
2. Implementar IMessageConsumer no ContratacaoService
3. Configurar Dead Letter Queues
4. Implementar retry policies
5. Adicionar monitoramento e alertas

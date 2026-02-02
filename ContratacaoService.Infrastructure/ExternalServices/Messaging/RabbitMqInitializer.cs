using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ContratacaoService.Infrastructure.ExternalServices.Messaging;

public class RabbitMqInitializer
{
    private readonly IConnection _connection;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqInitializer> _logger;

    public RabbitMqInitializer(
        IConnection connection,
        IConfiguration configuration,
        ILogger<RabbitMqInitializer> logger)
    {
        _connection = connection;
        _configuration = configuration;
        _logger = logger;
    }

    public void Initialize()
    {
        try
        {
            using var channel = _connection.CreateModel();

            var exchange = _configuration["RabbitMQ:Exchanges:Contratos"] ?? "contratos.exchange";
            var contratoCriadoQueue = _configuration["RabbitMQ:Queues:ContratoCriado"] ?? "contrato.criado.queue";

            // Declarar exchange
            channel.ExchangeDeclare(
                exchange: exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            _logger.LogInformation("Exchange {Exchange} declarado no RabbitMQ", exchange);

            // Declarar queue para ContratoCriado
            channel.QueueDeclare(
                queue: contratoCriadoQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _logger.LogInformation("Queue {Queue} declarada no RabbitMQ", contratoCriadoQueue);

            // Bind queue ao exchange
            channel.QueueBind(
                queue: contratoCriadoQueue,
                exchange: exchange,
                routingKey: "contrato.criado");

            _logger.LogInformation(
                "Queue {Queue} vinculada ao exchange {Exchange} com routing key 'contrato.criado'",
                contratoCriadoQueue,
                exchange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar RabbitMQ");
            throw;
        }
    }
}

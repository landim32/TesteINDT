using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ContratacaoService.Infrastructure.ExternalServices.Messaging;

public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConnection connection, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _logger = logger;
    }

    public Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Mensagem publicada no exchange {Exchange} com routing key {RoutingKey}", exchange, routingKey);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}

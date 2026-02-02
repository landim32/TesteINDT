using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ContratacaoService.Infrastructure.ExternalServices.Messaging;

public class RabbitMqConsumer : IMessageConsumer, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private EventingBasicConsumer? _consumer;

    public RabbitMqConsumer(IConnection connection, ILogger<RabbitMqConsumer> logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _logger = logger;
    }

    public Task<T?> ConsumeAsync<T>(string queue, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _channel.BasicGet(queue, autoAck: true);
            if (result == null)
                return Task.FromResult<T?>(default);

            var json = Encoding.UTF8.GetString(result.Body.ToArray());
            var message = JsonSerializer.Deserialize<T>(json);

            _logger.LogInformation("Mensagem consumida da fila {Queue}", queue);
            return Task.FromResult(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consumir mensagem do RabbitMQ");
            throw;
        }
    }

    public void StartConsuming<T>(string queue, Func<T, Task> handler)
    {
        _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += async (model, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);

                if (message != null)
                {
                    await handler(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Mensagem processada da fila {Queue}", queue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila {Queue}", queue);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: queue, autoAck: false, consumer: _consumer);
        _logger.LogInformation("Iniciado consumo da fila {Queue}", queue);
    }

    public void StopConsuming()
    {
        _logger.LogInformation("Parando consumo de mensagens");
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
    }
}

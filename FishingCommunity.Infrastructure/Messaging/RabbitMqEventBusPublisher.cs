using System.Text;
using System.Text.Json;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FishingCommunity.Infrastructure.Messaging;

public class RabbitMqEventBusPublisher : IEventBusPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqEventBusPublisher> _logger;

    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMqEventBusPublisher(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqEventBusPublisher> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task PublishAsync<TMessage>(string routingKey, TMessage message, CancellationToken cancellationToken = default) where TMessage : class
    {
        try
        {
            var channel = await GetOrCreateChannelAsync(cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true, // Survives a RabbitMQ restart — messages aren't lost if the broker goes down.
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            // Publishing failures should never take down the primary request/operation
            // that triggered them (e.g. approving a booking shouldn't fail just because
            // the message broker is temporarily unreachable) — log and swallow, same
            // "best effort" principle as email/notifications elsewhere in this codebase.
            _logger.LogError(ex, "Failed to publish event with routing key {RoutingKey}", routingKey);
        }
    }

    private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Declaring the exchange here (idempotent — safe to call every time) ensures
            // it exists before the first publish, without requiring a separate manual
            // setup step. "Topic" type enables the routing-key pattern matching described
            // earlier (e.g. consumers binding to "notification.#").
            await _channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: true, // Exchange survives a broker restart.
                autoDelete: false,
                cancellationToken: cancellationToken);

            return _channel;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
        }
    }
}
using System.Text;
using System.Text.Json;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Application.Features.Notifications.IntegrationEvents;
using FishingCommunity.Domain.Enums;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FishingCommunity.NotificationWorker;

public class NotificationConsumerService : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationConsumerService> _logger;

    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationConsumerService(
        IOptions<RabbitMqSettings> settings,
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationConsumerService> logger)
    {
        _settings = settings.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMqAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await HandleMessageAsync(eventArgs, stoppingToken);
        };

        await _channel!.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false, // Manual ack — we only confirm the message once it's fully processed.
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("Notification consumer started, listening on queue '{QueueName}'.", _settings.QueueName);

        // Keep the background service alive until the host shuts down —
        // the actual message handling happens in the ReceivedAsync event above.
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task InitializeRabbitMqAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(_settings.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);

        // Declaring the queue here too (idempotent) — the Worker can start up
        // completely independently of the API and everything still works, since
        // whichever process starts first creates the exchange/queue.
        await _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: _settings.RoutingKeyPattern,
            cancellationToken: cancellationToken);

        // Ensures this consumer only gets 1 unacknowledged message at a time —
        // prevents it from grabbing a huge backlog into memory at once if the
        // queue has a large number of pending messages.
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken stoppingToken)
    {
        var routingKey = eventArgs.RoutingKey;
        var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        _logger.LogInformation("Received message with routing key {RoutingKey}", routingKey);

        try
        {
            // Each message gets its own DI scope — mirrors how a single HTTP request
            // gets its own scope in the API, giving us a fresh DbContext/UnitOfWork
            // per message instead of sharing one across the whole worker's lifetime.
            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            await RouteMessageAsync(routingKey, json, notificationService, stoppingToken);

            await _channel!.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message with routing key {RoutingKey}. Message: {Message}", routingKey, json);

            // Reject without requeueing — a message that fails to process (e.g. due to
            // a bug or malformed payload) would otherwise loop forever between RabbitMQ
            // and this consumer. In a production system, this is where you'd route it
            // to a Dead Letter Queue instead of discarding it outright.
            await _channel!.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
        }
    }

    private static async Task RouteMessageAsync(string routingKey, string json, INotificationService notificationService, CancellationToken cancellationToken)
    {
        switch (routingKey)
        {
            case "notification.trip.booking.requested":
                {
                    var evt = Deserialize<TripBookingRequestedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.OrganizerId, NotificationType.TripBookingRequested,
                        "New booking request",
                        $"Someone requested to book a seat on your trip \"{evt.TripTitle}\".",
                        evt.TripId, cancellationToken);
                    break;
                }

            case "notification.trip.booking.approved":
                {
                    var evt = Deserialize<TripBookingApprovedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.UserId, NotificationType.TripBookingApproved,
                        "Booking approved!",
                        "Your booking request has been approved. Get ready for your trip!",
                        evt.TripId, cancellationToken);
                    break;
                }

            case "notification.trip.booking.rejected":
                {
                    var evt = Deserialize<TripBookingRejectedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.UserId, NotificationType.TripBookingRejected,
                        "Booking not approved",
                        "Unfortunately, your booking request was not approved this time.",
                        evt.TripId, cancellationToken);
                    break;
                }

            case "notification.trip.cancelled":
                {
                    var evt = Deserialize<TripCancelledIntegrationEvent>(json);
                    foreach (var userId in evt.AffectedUserIds)
                    {
                        await notificationService.CreateNotificationAsync(
                            userId, NotificationType.TripCancelled,
                            "Trip cancelled",
                            evt.Reason is not null ? $"A trip you booked was cancelled. Reason: {evt.Reason}" : "A trip you booked was cancelled.",
                            evt.TripId, cancellationToken);
                    }
                    break;
                }

            case "notification.post.commented":
                {
                    var evt = Deserialize<PostCommentedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.PostAuthorId, NotificationType.PostCommented,
                        "New comment on your post",
                        "Someone commented on your post.",
                        evt.PostId, cancellationToken);
                    break;
                }

            case "notification.post.liked":
                {
                    var evt = Deserialize<PostLikedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.PostAuthorId, NotificationType.PostLiked,
                        "New like",
                        "Someone liked your post.",
                        evt.PostId, cancellationToken);
                    break;
                }

            case "notification.user.followed":
                {
                    var evt = Deserialize<UserFollowedIntegrationEvent>(json);
                    await notificationService.CreateNotificationAsync(
                        evt.FollowedId, NotificationType.UserFollowed,
                        "New follower",
                        "You have a new follower!",
                        evt.FollowerId, cancellationToken);
                    break;
                }

            default:
                // Unknown routing key — log and ignore rather than crash, in case a
                // future event type is published before this consumer is updated to
                // handle it (forward compatibility).
                break;
        }
    }

    private static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException($"Failed to deserialize message to {typeof(T).Name}.");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.CloseAsync(cancellationToken);
        if (_connection is not null) await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
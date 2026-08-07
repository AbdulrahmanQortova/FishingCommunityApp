namespace FishingCommunity.Application.Common.Interfaces;

public interface IEventBusPublisher
{
    /// <summary>
    /// Publishes an integration event to the message broker for asynchronous,
    /// cross-process handling — distinct from MediatR's in-process domain events.
    /// </summary>
    /// <param name="routingKey">
    /// Dot-separated topic used for routing, e.g. "notification.trip.booking.approved".
    /// Consumers bind to patterns like "notification.#" to receive matching events.
    /// </param>
    Task PublishAsync<TMessage>(string routingKey, TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
}
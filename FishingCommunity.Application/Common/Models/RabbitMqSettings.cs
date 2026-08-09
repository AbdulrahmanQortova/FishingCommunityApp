namespace FishingCommunity.Application.Common.Models;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMqSettings";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "fishing_community_events";

    // Used by the consumer (Worker Service) only — not needed by the publisher (API).
    public string QueueName { get; set; } = "notifications_queue";
    public string RoutingKeyPattern { get; set; } = "notification.#";
}
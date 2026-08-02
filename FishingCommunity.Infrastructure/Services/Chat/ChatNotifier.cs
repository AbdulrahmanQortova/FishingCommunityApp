using FishingCommunity.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FishingCommunity.Infrastructure.Services.Chat;

// Generic Hub reference — decouples Infrastructure from the API project's concrete
// ChatHub class. The actual Hub type is registered/mapped in the API layer.
public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<Hub> _hubContext;
    private readonly IChatConnectionTracker _connectionTracker;

    public ChatNotifier(IHubContext<Hub> hubContext, IChatConnectionTracker connectionTracker)
    {
        _hubContext = hubContext;
        _connectionTracker = connectionTracker;
    }

    public async Task NotifyMessageReceivedAsync(Guid recipientUserId, ChatMessageNotification message, CancellationToken cancellationToken = default)
    {
        var connections = _connectionTracker.GetConnections(recipientUserId);
        if (connections.Count == 0) return;

        await _hubContext.Clients.Clients(connections).SendAsync("MessageReceived", message, cancellationToken);
    }

    public async Task NotifyTypingAsync(Guid recipientUserId, Guid conversationId, Guid typingUserId, CancellationToken cancellationToken = default)
    {
        var connections = _connectionTracker.GetConnections(recipientUserId);
        if (connections.Count == 0) return;

        await _hubContext.Clients.Clients(connections).SendAsync("Typing", conversationId, typingUserId, cancellationToken);
    }

    public async Task NotifyMessageReadAsync(Guid recipientUserId, Guid conversationId, Guid readByUserId, CancellationToken cancellationToken = default)
    {
        var connections = _connectionTracker.GetConnections(recipientUserId);
        if (connections.Count == 0) return;

        await _hubContext.Clients.Clients(connections).SendAsync("MessagesRead", conversationId, readByUserId, cancellationToken);
    }

    public Task<bool> IsUserOnlineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_connectionTracker.IsOnline(userId));
    }
}
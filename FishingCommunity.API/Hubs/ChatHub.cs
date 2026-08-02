using FishingCommunity.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FishingCommunity.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatConnectionTracker _connectionTracker;

    public ChatHub(IChatConnectionTracker connectionTracker)
    {
        _connectionTracker = connectionTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        if (userId is not null)
        {
            _connectionTracker.AddConnection(userId.Value, Context.ConnectionId);
            await Clients.Others.SendAsync("UserOnline", userId.Value);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();

        if (userId is not null)
        {
            var isNowOffline = _connectionTracker.RemoveConnection(userId.Value, Context.ConnectionId);

            if (isNowOffline)
            {
                await Clients.Others.SendAsync("UserOffline", userId.Value);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyTyping(Guid conversationId, Guid recipientUserId)
    {
        var senderId = GetUserId();
        if (senderId is null) return;

        var recipientConnections = _connectionTracker.GetConnections(recipientUserId);

        if (recipientConnections.Count > 0)
        {
            await Clients.Clients(recipientConnections).SendAsync("Typing", conversationId, senderId.Value);
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
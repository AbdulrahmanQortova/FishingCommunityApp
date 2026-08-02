using System.Collections.Concurrent;
using FishingCommunity.Application.Common.Interfaces;

namespace FishingCommunity.Infrastructure.Services.Chat;

public class ChatConnectionTracker : IChatConnectionTracker
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> UserConnections = new();
    private static readonly object LockObject = new();

    public void AddConnection(Guid userId, string connectionId)
    {
        lock (LockObject)
        {
            var connections = UserConnections.GetOrAdd(userId, _ => new HashSet<string>());
            connections.Add(connectionId);
        }
    }

    public bool RemoveConnection(Guid userId, string connectionId)
    {
        lock (LockObject)
        {
            if (!UserConnections.TryGetValue(userId, out var connections))
            {
                return false;
            }

            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                UserConnections.TryRemove(userId, out _);
                return true; // Now fully offline.
            }

            return false;
        }
    }

    public List<string> GetConnections(Guid userId)
    {
        lock (LockObject)
        {
            return UserConnections.TryGetValue(userId, out var connections)
                ? connections.ToList()
                : new List<string>();
        }
    }

    public bool IsOnline(Guid userId)
    {
        lock (LockObject)
        {
            return UserConnections.ContainsKey(userId);
        }
    }
}
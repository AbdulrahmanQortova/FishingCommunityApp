namespace FishingCommunity.Application.Common.Interfaces;

public interface IChatConnectionTracker
{
    void AddConnection(Guid userId, string connectionId);
    bool RemoveConnection(Guid userId, string connectionId); // Returns true if the user is now fully offline.
    List<string> GetConnections(Guid userId);
    bool IsOnline(Guid userId);
}
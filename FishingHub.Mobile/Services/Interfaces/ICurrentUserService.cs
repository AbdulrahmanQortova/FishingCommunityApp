using System.ComponentModel;
using FishingHub.Mobile.Models;

namespace FishingHub.Mobile.Services.Interfaces;

public interface ICurrentUserService : INotifyPropertyChanged
{
    CurrentUser? User { get; }
    bool IsAuthenticated { get; }

    void SetUser(CurrentUser user);
    Task ClearAsync();

    /// <summary>
    /// Attempts to restore the current user from persisted storage on app startup
    /// (if a valid session exists). Returns true if a user was restored.
    /// </summary>
    Task<bool> TryRestoreSessionAsync();
}
using FishingCommunity.Application.Common.Interfaces;

namespace FishingCommunity.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
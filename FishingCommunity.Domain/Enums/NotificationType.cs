namespace FishingCommunity.Domain.Enums;

public enum NotificationType
{
    TripBookingRequested = 1,
    TripBookingApproved = 2,
    TripBookingRejected = 3,
    TripCancelled = 4,
    PostCommented = 5,
    PostLiked = 6,
    UserFollowed = 7,
    System = 8
}
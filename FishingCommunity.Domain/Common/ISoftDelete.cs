namespace FishingCommunity.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedDate { get; set; }
    Guid? DeletedBy { get; set; }
}
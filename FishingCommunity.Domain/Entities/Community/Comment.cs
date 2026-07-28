using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Community;

public class Comment : BaseAuditableEntity
{
    public Guid PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;

    public Guid? ParentCommentId { get; private set; }

    public bool IsEdited { get; private set; }
    public bool IsRemoved { get; private set; }

    private Comment() { } // EF Core

    internal Comment(Guid postId, Guid userId, string content, Guid? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new BusinessRuleValidationException("Comment content cannot be empty.");
        }

        PostId = postId;
        UserId = userId;
        Content = content;
        ParentCommentId = parentCommentId;
    }

    internal void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new BusinessRuleValidationException("Comment content cannot be empty.");
        }

        Content = content;
        IsEdited = true;
    }

    internal void MarkAsDeleted()
    {
        IsRemoved = true;
        Content = "[deleted]";
    }
}
using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Events.Community;
using FishingCommunity.Domain.Exceptions;
using System.Xml.Linq;

namespace FishingCommunity.Domain.Entities.Community;

public class Post : BaseAuditableEntity, IAggregateRoot
{
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = string.Empty;

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    public bool IsEdited { get; private set; }

    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<PostReaction> _reactions = new();
    public IReadOnlyCollection<PostReaction> Reactions => _reactions.AsReadOnly();

    public int LikesCount => _reactions.Count(r => r.Type == Enums.ReactionType.Like);
    public int DislikesCount => _reactions.Count(r => r.Type == Enums.ReactionType.Dislike);
    public int CommentsCount => _comments.Count;

    private Post() { } // EF Core

    public Post(Guid authorId, string content, IEnumerable<string>? photoUrls = null)
    {
        if (string.IsNullOrWhiteSpace(content) && (photoUrls is null || !photoUrls.Any()))
        {
            throw new BusinessRuleValidationException("A post must have content or at least one photo.");
        }

        AuthorId = authorId;
        Content = content;

        if (photoUrls is not null)
        {
            _photoUrls.AddRange(photoUrls);
        }

        AddDomainEvent(new PostCreatedEvent(Id, authorId));
    }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content) && _photoUrls.Count == 0)
        {
            throw new BusinessRuleValidationException("A post must have content or at least one photo.");
        }

        Content = content;
        IsEdited = true;
    }

    public void AddPhoto(string url) => _photoUrls.Add(url);
    public void RemovePhoto(string url) => _photoUrls.Remove(url);

    public Comment AddComment(Guid userId, string content, Guid? parentCommentId = null)
    {
        if (parentCommentId is not null && !_comments.Any(c => c.Id == parentCommentId))
        {
            throw new NotFoundException(nameof(Comment), parentCommentId.Value);
        }

        var comment = new Comment(Id, userId, content, parentCommentId);
        _comments.Add(comment);

        AddDomainEvent(new PostCommentedEvent(Id, comment.Id, userId, AuthorId));

        return comment;
    }

    public void RemoveComment(Guid commentId, Guid requestingUserId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);

        if (comment is null)
        {
            throw new NotFoundException(nameof(Comment), commentId);
        }

        // Either the comment author or the post author can remove a comment.
        if (comment.UserId != requestingUserId && AuthorId != requestingUserId)
        {
            throw new BusinessRuleValidationException("You are not authorized to remove this comment.");
        }

        comment.MarkAsDeleted();
    }

    public void React(Guid userId, Enums.ReactionType type)
    {
        var existing = _reactions.FirstOrDefault(r => r.UserId == userId);

        if (existing is not null)
        {
            if (existing.Type == type)
            {
                // Same reaction tapped again -> remove it (toggle off).
                _reactions.Remove(existing);
                return;
            }

            // Different reaction -> switch it (e.g. Like -> Dislike).
            existing.ChangeType(type);
            return;
        }

        var reaction = new PostReaction(Id, userId, type);
        _reactions.Add(reaction);

        if (type == Enums.ReactionType.Like)
        {
            AddDomainEvent(new PostLikedEvent(Id, userId, AuthorId));
        }
    }

    public void RemoveReaction(Guid userId)
    {
        var existing = _reactions.FirstOrDefault(r => r.UserId == userId);
        if (existing is not null)
        {
            _reactions.Remove(existing);
        }
    }
}
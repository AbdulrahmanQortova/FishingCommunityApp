using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace FishingCommunity.UnitTests.Domain.Community;

public class PostTests
{
    [Fact]
    public void Constructor_WithContentOnly_CreatesPostSuccessfully()
    {
        // Act
        var post = new Post(Guid.NewGuid(), "Great day fishing!");

        // Assert
        post.Content.Should().Be("Great day fishing!");
        post.IsEdited.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNoContentAndNoPhotos_ThrowsBusinessRuleValidationException()
    {
        // Act
        var act = () => new Post(Guid.NewGuid(), "", null);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*must have content or at least one photo*");
    }

    [Fact]
    public void Constructor_WithPhotosOnlyNoContent_CreatesPostSuccessfully()
    {
        // Act
        var post = new Post(Guid.NewGuid(), "", new[] { "https://example.com/photo.jpg" });

        // Assert
        post.PhotoUrls.Should().ContainSingle();
    }

    [Fact]
    public void Edit_UpdatesContentAndMarksAsEdited()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Original content");

        // Act
        post.Edit("Updated content");

        // Assert
        post.Content.Should().Be("Updated content");
        post.IsEdited.Should().BeTrue();
    }

    [Fact]
    public void React_FirstTimeLike_AddsLikeReaction()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var userId = Guid.NewGuid();

        // Act
        post.React(userId, ReactionType.Like);

        // Assert
        post.LikesCount.Should().Be(1);
        post.DislikesCount.Should().Be(0);
    }

    [Fact]
    public void React_SameReactionTwice_TogglesItOff()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var userId = Guid.NewGuid();
        post.React(userId, ReactionType.Like);

        // Act
        post.React(userId, ReactionType.Like); // Same reaction again.

        // Assert
        post.LikesCount.Should().Be(0);
        post.Reactions.Should().BeEmpty();
    }

    [Fact]
    public void React_DifferentReactionType_SwitchesReaction()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var userId = Guid.NewGuid();
        post.React(userId, ReactionType.Like);

        // Act
        post.React(userId, ReactionType.Dislike);

        // Assert
        post.LikesCount.Should().Be(0);
        post.DislikesCount.Should().Be(1);
    }

    [Fact]
    public void AddComment_ToPost_IncreasesCommentsCount()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");

        // Act
        var comment = post.AddComment(Guid.NewGuid(), "Nice catch!");

        // Assert
        post.CommentsCount.Should().Be(1);
        comment.Content.Should().Be("Nice catch!");
    }

    [Fact]
    public void AddComment_AsReplyToNonExistentComment_ThrowsNotFoundException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var fakeParentId = Guid.NewGuid();

        // Act
        var act = () => post.AddComment(Guid.NewGuid(), "Reply", fakeParentId);

        // Assert
        act.Should().Throw<NotFoundException>();
    }

    [Fact]
    public void RemoveComment_ByCommentAuthor_MarksAsDeleted()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var commenterId = Guid.NewGuid();
        var comment = post.AddComment(commenterId, "My comment");

        // Act
        post.RemoveComment(comment.Id, commenterId);

        // Assert
        comment.IsRemoved.Should().BeTrue();
        comment.Content.Should().Be("[deleted]");
    }

    [Fact]
    public void RemoveComment_ByPostAuthor_MarksAsDeleted()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var post = new Post(authorId, "Content");
        var comment = post.AddComment(Guid.NewGuid(), "Someone else's comment");

        // Act
        post.RemoveComment(comment.Id, authorId);

        // Assert
        comment.IsRemoved.Should().BeTrue();
    }

    [Fact]
    public void RemoveComment_ByUnrelatedUser_ThrowsBusinessRuleValidationException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), "Content");
        var comment = post.AddComment(Guid.NewGuid(), "Comment");
        var strangerId = Guid.NewGuid();

        // Act
        var act = () => post.RemoveComment(comment.Id, strangerId);

        // Assert
        act.Should().Throw<BusinessRuleValidationException>()
            .WithMessage("*not authorized*");
    }
}
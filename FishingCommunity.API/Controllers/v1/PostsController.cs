using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Community.Posts.Commands.AddComment;
using FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;
using FishingCommunity.Application.Features.Community.Posts.Commands.DeletePost;
using FishingCommunity.Application.Features.Community.Posts.Commands.EditPost;
using FishingCommunity.Application.Features.Community.Posts.Commands.ReactToPost;
using FishingCommunity.Application.Features.Community.Posts.Commands.RemoveComment;
using FishingCommunity.Application.Features.Community.Posts.Commands.ReportPost;
using FishingCommunity.Application.Features.Community.Posts.Commands.ToggleSavePost;
using FishingCommunity.Application.Features.Community.Posts.Queries.GetFeed;
using FishingCommunity.Application.Features.Community.Posts.Queries.GetMySavedPosts;
using FishingCommunity.Application.Features.Community.Posts.Queries.GetPostDetails;
using FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/posts")]
public class PostsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public PostsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetFeedQuery
        {
            RequestingUserId = _currentUserService.UserId, // null if anonymous
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{postId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetails(Guid postId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPostDetailsQuery { PostId = postId }, cancellationToken);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpGet("saved")]
    [Authorize]
    public async Task<IActionResult> GetMySavedPosts(CancellationToken cancellationToken)
    {
        var query = new GetMySavedPostsQuery { UserId = _currentUserService.UserId!.Value };
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePostRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreatePostCommand
        {
            AuthorId = _currentUserService.UserId!.Value,
            Content = request.Content,
            PhotoUrls = request.PhotoUrls
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{postId:guid}")]
    [Authorize]
    public async Task<IActionResult> Edit(Guid postId, [FromBody] EditPostRequestDto request, CancellationToken cancellationToken)
    {
        var command = new EditPostCommand
        {
            PostId = postId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Content = request.Content
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{postId:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid postId, CancellationToken cancellationToken)
    {
        var command = new DeletePostCommand
        {
            PostId = postId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{postId:guid}/comments")]
    [Authorize]
    public async Task<IActionResult> AddComment(Guid postId, [FromBody] AddCommentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand
        {
            PostId = postId,
            UserId = _currentUserService.UserId!.Value,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{postId:guid}/comments/{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> RemoveComment(Guid postId, Guid commentId, CancellationToken cancellationToken)
    {
        var command = new RemoveCommentCommand
        {
            PostId = postId,
            CommentId = commentId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{postId:guid}/react")]
    [Authorize]
    public async Task<IActionResult> React(Guid postId, [FromBody] ReactToPostRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ReactToPostCommand
        {
            PostId = postId,
            UserId = _currentUserService.UserId!.Value,
            Type = request.Type
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{postId:guid}/save")]
    [Authorize]
    public async Task<IActionResult> ToggleSave(Guid postId, CancellationToken cancellationToken)
    {
        var command = new ToggleSavePostCommand
        {
            PostId = postId,
            UserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{postId:guid}/report")]
    [Authorize]
    public async Task<IActionResult> Report(Guid postId, [FromBody] ReportPostRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ReportPostCommand
        {
            PostId = postId,
            ReportedByUserId = _currentUserService.UserId!.Value,
            Reason = request.Reason,
            AdditionalDetails = request.AdditionalDetails
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
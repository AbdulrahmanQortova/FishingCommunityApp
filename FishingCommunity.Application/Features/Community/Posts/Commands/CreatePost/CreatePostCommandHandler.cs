using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<CreatePostResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatePostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post(request.AuthorId, request.Content, request.PhotoUrls);

        await _unitOfWork.Repository<Post>().AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreatePostResponse
        {
            PostId = post.Id,
            Content = post.Content,
            CreatedDate = post.CreatedDate
        };

        return Result<CreatePostResponse>.Success(response, "Post created successfully.");
    }
}
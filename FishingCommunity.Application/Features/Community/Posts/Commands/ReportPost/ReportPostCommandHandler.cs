using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReportPost;

public class ReportPostCommandHandler : IRequestHandler<ReportPostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReportPostCommand request, CancellationToken cancellationToken)
    {
        var postExists = await _unitOfWork.Repository<Post>().AnyAsync(p => p.Id == request.PostId, cancellationToken);

        if (!postExists)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        var alreadyReported = await _unitOfWork.Repository<PostReport>()
            .AnyAsync(r => r.PostId == request.PostId && r.ReportedByUserId == request.ReportedByUserId, cancellationToken);

        if (alreadyReported)
        {
            return Result.Failure("You have already reported this post.");
        }

        var report = new PostReport(request.PostId, request.ReportedByUserId, request.Reason, request.AdditionalDetails);

        await _unitOfWork.Repository<PostReport>().AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Post reported. Our team will review it shortly.");
    }
}
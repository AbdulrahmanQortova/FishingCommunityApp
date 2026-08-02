using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Reports.Commands.ResolveReport;

public class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ResolveReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Repository<PostReport>().GetByIdAsync(request.ReportId, cancellationToken);

        if (report is null)
        {
            throw new NotFoundException(nameof(PostReport), request.ReportId);
        }

        report.Resolve(request.AdminUserId, request.NewStatus, request.ResolutionNotes);
        _unitOfWork.Repository<PostReport>().Update(report);

        if (request.NewStatus == ReportStatus.ActionTaken && request.DeletePost)
        {
            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(report.PostId, cancellationToken);

            if (post is not null)
            {
                _unitOfWork.Repository<Post>().Remove(post); // Soft delete.
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Report resolved successfully.");
    }
}
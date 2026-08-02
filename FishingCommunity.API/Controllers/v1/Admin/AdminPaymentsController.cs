using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Payments.Commands.ApprovePayment;
using FishingCommunity.Application.Features.Payments.Commands.RejectPayment;
using FishingCommunity.Application.Features.Payments.Queries.GetPendingPaymentsForReview;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/payments")]
[Authorize(Roles = Roles.Administrator)]
public class AdminPaymentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AdminPaymentsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet("pending-review")]
    public async Task<IActionResult> GetPendingReview([FromQuery] GetPendingPaymentsForReviewQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{paymentId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid paymentId, CancellationToken cancellationToken)
    {
        var command = new ApprovePaymentCommand { PaymentId = paymentId, AdminUserId = _currentUserService.UserId!.Value };
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{paymentId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid paymentId, [FromBody] RejectPaymentRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RejectPaymentCommand
        {
            PaymentId = paymentId,
            AdminUserId = _currentUserService.UserId!.Value,
            Reason = request.Reason
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Payments.Commands.SubmitTransferProof;
using FishingCommunity.Application.Features.Payments.Queries.GetMyPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public PaymentsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyPaymentsQuery { UserId = _currentUserService.UserId!.Value }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{paymentId:guid}/submit-proof")]
    public async Task<IActionResult> SubmitProof(Guid paymentId, [FromBody] SubmitTransferProofRequestDto request, CancellationToken cancellationToken)
    {
        var command = new SubmitTransferProofCommand
        {
            PaymentId = paymentId,
            RequestingUserId = _currentUserService.UserId!.Value,
            SenderPhoneOrHandle = request.SenderPhoneOrHandle,
            ProofUrl = request.ProofUrl
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
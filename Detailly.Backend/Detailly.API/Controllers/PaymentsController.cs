using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;
using Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundPayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Detailly.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAppCurrentUser _currentUser;

    public PaymentsController(IMediator mediator, IAppCurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    // -------------------------------
    // 1) TOP UP WALLET
    // -------------------------------
    [HttpPost("wallet/top-up")]
    [Authorize]
    public async Task<IActionResult> TopUpWallet([FromBody] TopUpWalletCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    // -------------------------------
    // 2) PAY BOOKING WITH WALLET
    // -------------------------------
    [HttpPost("bookings/{bookingId}/wallet")]
    [Authorize]
    public async Task<IActionResult> PayBookingWithWallet(int bookingId)
    {
        if (_currentUser.ApplicationUserId is null)
            return Unauthorized();

        var userId = _currentUser.ApplicationUserId.Value;

        await _mediator.Send(new PayBookingWithWalletCommand(userId, bookingId));
        return Ok();
    }

    // -------------------------------
    // 3) CREATE CARD PAYMENT INTENT
    // -------------------------------
    [HttpPost("bookings/{bookingId}/card-intent")]
    [Authorize]
    public async Task<IActionResult> CreateCardIntent(int bookingId)
    {
        if (_currentUser.ApplicationUserId is null)
            return Unauthorized();

        var userId = _currentUser.ApplicationUserId.Value;

        var result = await _mediator.Send(
            new CreateCardPaymentIntentCommand(userId, bookingId)
        );

        return Ok(result);
    }

    // -------------------------------
    // 4) REFUND PAYMENT (admin)
    // -------------------------------
    [HttpPost("{paymentId}/refund")]
    [Authorize] // later restrict to Admin/Manager
    public async Task<IActionResult> RefundPayment(int paymentId)
    {
        await _mediator.Send(new RefundPaymentCommand(paymentId));
        return Ok();
    }
}

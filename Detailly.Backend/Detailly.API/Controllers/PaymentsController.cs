// FILE: Detailly.API/Controllers/PaymentsController.cs
using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Payment.Card.Commands.CreateCardPaymentIntent;
using Detailly.Application.Modules.Payment.Card.Commands.RefundCardPayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUpByCard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController(ISender sender, IAppCurrentUser currentUser) : ControllerBase
{
    // -------------------------------
    // 1) TOP UP WALLET (instant/internal)
    // -------------------------------
    [HttpPost("wallet/top-up")]
    [Authorize]
    public async Task<IActionResult> TopUpWallet([FromBody] TopUpWalletCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return Ok();
    }

    // -------------------------------
    // 2) PAY BOOKING WITH WALLET
    // -------------------------------
    [HttpPost("bookings/{bookingId:int}/wallet")]
    [Authorize]
    public async Task<IActionResult> PayBookingWithWallet(int bookingId, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return Unauthorized();

        await sender.Send(new PayBookingWithWalletCommand(currentUser.ApplicationUserId.Value, bookingId), ct);
        return Ok();
    }

    // -------------------------------
    // 3) CREATE CARD PAYMENT INTENT (booking)
    // -------------------------------
    [HttpPost("bookings/{bookingId:int}/card-intent")]
    [Authorize]
    public async Task<ActionResult<CreateCardPaymentIntentResult>> CreateCardIntent(int bookingId, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return Unauthorized();

        var result = await sender.Send(new CreateCardPaymentIntentCommand(currentUser.ApplicationUserId.Value, bookingId), ct);
        return Ok(result);
    }

    // -------------------------------
    // 4) CREATE CARD PAYMENT INTENT (wallet top-up)
    // -------------------------------
    [HttpPost("wallet/top-up/card-intent")]
    [Authorize]
    public async Task<ActionResult<CreateWalletTopUpCardIntentResult>> CreateWalletTopUpCardIntent([FromBody] WalletTopUpIntentRequest req, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return Unauthorized();

        var result = await sender.Send(
            new CreateWalletTopUpCardIntentCommand(currentUser.ApplicationUserId.Value, req.Amount, req.Description),
            ct);

        return Ok(result);
    }

    // -------------------------------
    // 5) REFUND WALLET PAYMENT (admin)
    // -------------------------------
    [HttpPost("{paymentId:int}/refund-wallet")]
    [Authorize] // later restrict to Admin/Manager policy
    public async Task<IActionResult> RefundWalletPayment(int paymentId, [FromBody] RefundRequest req, CancellationToken ct)
    {
        await sender.Send(new RefundWalletPaymentCommand(paymentId, req.Amount), ct);
        return Ok();
    }

    // -------------------------------
    // 6) REFUND CARD PAYMENT (admin)
    // -------------------------------
    [HttpPost("{paymentId:int}/refund-card")]
    [Authorize] // later restrict to Admin/Manager policy
    public async Task<IActionResult> RefundCardPayment(int paymentId, [FromBody] RefundRequest req, CancellationToken ct)
    {
        await sender.Send(new RefundCardPaymentCommand(paymentId, req.Amount), ct);
        return Ok();
    }
}

public record WalletTopUpIntentRequest(decimal Amount, string? Description);
public record RefundRequest(decimal Amount);
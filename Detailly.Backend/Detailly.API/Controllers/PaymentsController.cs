using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Payment.Card.Commands.CreateBookingPaymentIntent;
using Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;
using Detailly.Application.Modules.Payment.Stripe.Commands.CreateOrderPaymentIntent;
using Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;
using Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;
using Detailly.Application.Modules.Payment.Wallet.Commands.TopUpByCard;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController(ISender sender, IAppCurrentUser currentUser) : ControllerBase
{
    // -------------------------------
    // 1) TOP UP WALLET (instant/internal)
    // -------------------------------
    [HttpPost("wallet/top-up")]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<IActionResult> TopUpWallet([FromBody] TopUpWalletCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
        return Ok();
    }

    // -------------------------------
    // 2) PAY BOOKING WITH WALLET
    // -------------------------------
    [HttpPost("bookings/wallet/{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
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
    [HttpPost("bookings/card-intent/{bookingId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<CreateBookingPaymentIntentResult>> CreateCardIntent(int bookingId, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return Unauthorized();

        var result = await sender.Send(new CreateBookingPaymentIntentCommand(currentUser.ApplicationUserId.Value, bookingId), ct);
        return Ok(result);
    }

    // -------------------------------
    // 4) CREATE CARD PAYMENT INTENT (order)
    // -------------------------------
    [HttpPost("orders/card-intent/{orderId:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<CreateOrderPaymentIntentResult>> CreateOrderCardIntent(int orderId, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return Unauthorized();

        // Ownership is enforced in the handler
        var result = await sender.Send(new CreateOrderPaymentIntentCommand(orderId), ct);
        return Ok(result);
    }

    // -------------------------------
    // 5) CREATE CARD PAYMENT INTENT (wallet top-up)
    // -------------------------------
    [HttpPost("wallet/top-up/card-intent")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
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
    // 6) REFUND WALLET PAYMENT (admin)
    // -------------------------------
    [HttpPost("refund-wallet/{paymentId:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]    // later restrict to Admin/Manager policy
    public async Task<IActionResult> RefundWalletPayment(int paymentId, [FromBody] RefundRequest req, CancellationToken ct)
    {
        await sender.Send(new RefundWalletPaymentCommand(paymentId, req.Amount), ct);
        return Ok();
    }

    // -------------------------------
    // 7) REFUND CARD PAYMENT (admin)
    // -------------------------------
    [HttpPost("refund-card/{paymentId:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]    // later restrict to Admin/Manager policy
    public async Task<IActionResult> RefundCardPayment(int paymentId, [FromBody] RefundRequest req, CancellationToken ct)
    {
        await sender.Send(new RefundStripePaymentCommand(paymentId, req.Amount), ct);
        return Ok();
    }
}

public record WalletTopUpIntentRequest(decimal Amount, string? Description);
public record RefundRequest(decimal Amount);
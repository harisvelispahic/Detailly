using MediatR;
using Microsoft.AspNetCore.Mvc;
using Detailly.Application.Modules.Payment.Card.Commands.HandleStripeWebhook;

namespace Detailly.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
[AllowAnonymous]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        var signature = Request.Headers["Stripe-Signature"].ToString();

        await _mediator.Send(new HandleStripeWebhookCommand(payload, signature));

        return Ok();
    }
}

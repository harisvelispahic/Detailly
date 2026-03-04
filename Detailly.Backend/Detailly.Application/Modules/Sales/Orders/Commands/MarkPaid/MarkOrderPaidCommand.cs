namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkPaid;

public sealed class MarkOrderPaidCommand : IRequest
{
    public int Id { get; set; }

    // optional payment metadata (safe to keep for now)
    public string? Provider { get; set; }                 // e.g. "Wallet", "Stripe"
    public string? ProviderTransactionId { get; set; }    // external reference
    public string? Description { get; set; }
}
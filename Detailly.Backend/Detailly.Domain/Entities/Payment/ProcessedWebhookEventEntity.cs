using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Payment;

public class ProcessedWebhookEventEntity : BaseEntity
{
    public required string EventId { get; set; }
}

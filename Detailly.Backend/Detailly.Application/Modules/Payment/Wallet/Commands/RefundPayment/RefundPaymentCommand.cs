public record RefundPaymentCommand(
    int PaymentTransactionId
) : IRequest<Unit>;

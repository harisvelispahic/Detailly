namespace Detailly.Application.Modules.Sales.Orders.Commands.ConfirmAfterPayment;

public record ConfirmOrderAfterPaymentCommand(int PaymentTransactionId) : IRequest<Unit>;
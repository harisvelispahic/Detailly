namespace Detailly.Domain.Common.Enums;
public enum TransactionType
{
    Deposit = 0,     // Funds deposited (e.g. user adds money to wallet)
    Withdrawal = 1,  // Funds withdrawn / deducted from wallet
    Payment = 2,     // Payment for an order or service
    Refund = 3       // Refund back to wallet or original payment method
}


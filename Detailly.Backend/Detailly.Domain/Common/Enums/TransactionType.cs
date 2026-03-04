namespace Detailly.Domain.Common.Enums;
public enum TransactionType
{
    Deposit = 1,     // Uplata sredstava (npr. korisnik dodaje novac)
    Withdrawal = 2,  // Isplata / povlačenje sredstava
    Payment = 3,     // Plaćanje narudžbe ili servisa
    Refund = 4       // Povrat novca
}


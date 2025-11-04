namespace Detailly.Domain.Common.Enums;
public enum TransactionType
{
    Deposit = 0,     // Uplata sredstava (npr. korisnik dodaje novac)
    Withdrawal = 1,  // Isplata / povlačenje sredstava
    Payment = 2,     // Plaćanje narudžbe ili servisa
    Refund = 3       // Povrat novca
}


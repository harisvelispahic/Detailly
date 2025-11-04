

namespace Detailly.Domain.Common.Enums;

public enum CartStatus
{
    Active = 0,     // Korisnik dodaje artikle, korpa još nije potvrđena
    CheckedOut = 1, // Korpa je potvrđena i pretvorena u Order
    Abandoned = 2,  // Korpa napuštena (korisnik nije završio proces)
    Cancelled = 3   // Korpa ručno otkazana
}


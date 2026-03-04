

namespace Detailly.Domain.Common.Enums;

public enum CartStatus
{
    Active = 1,     // User adds products, but hasn't started checkout yet
    Abandoned = 2,  // Cart abandoned, e.g. user left without checking out for a long time (e.g. 24+ hours)
}


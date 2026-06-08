namespace Detailly.Application.Abstractions;

public interface IReturnUrlValidator
{
    bool IsAllowed(string returnUrl);
}

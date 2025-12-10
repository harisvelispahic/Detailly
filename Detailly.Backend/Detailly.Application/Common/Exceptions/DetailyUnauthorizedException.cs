
namespace Detailly.Application.Common.Exceptions;
public sealed class DetaillyUnauthorizedException : Exception
{
    public DetaillyUnauthorizedException(string message) : base(message) { }
}

namespace Detailly.Application.Common.Exceptions;

public sealed class DetaillyNotFoundException : Exception
{
    public DetaillyNotFoundException(string message) : base(message) { }
}

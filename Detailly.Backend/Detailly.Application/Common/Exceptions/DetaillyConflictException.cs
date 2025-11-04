namespace Detailly.Application.Common.Exceptions;

public sealed class DetaillyConflictException : Exception
{
    public DetaillyConflictException(string message) : base(message) { }
}

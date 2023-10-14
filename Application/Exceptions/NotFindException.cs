namespace Application.Exceptions;

public sealed class NotFindException : Exception
{
    public NotFindException(string message) : base($"I couldn't find: '{message}'")
    { }
}
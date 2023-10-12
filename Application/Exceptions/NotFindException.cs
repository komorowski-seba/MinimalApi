namespace Application.Exceptions;

public class NotFindException : Exception
{
    public NotFindException(string message) : base($"I couldn't find: '{message}'")
    { }
}
namespace Octagram.Application.Exceptions;

public class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; private set; } = statusCode;
}
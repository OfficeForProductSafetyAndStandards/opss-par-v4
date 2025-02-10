namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Exceptions;

public class ServiceNotfoundException : Exception
{
    public ServiceNotfoundException(string message)
        : base($"Service: {message} not found")
    {
    }

    public ServiceNotfoundException(string message, Exception innerException)
        : base($"Service: {message} not found", innerException)
    {
    }
}

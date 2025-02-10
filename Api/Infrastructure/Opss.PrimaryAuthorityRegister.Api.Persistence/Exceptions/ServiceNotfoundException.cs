namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Exceptions;

public class ServiceNotFoundException : Exception
{
    public string ServiceName { get; }

    public ServiceNotFoundException(string serviceName)
        : base($"Service: {serviceName} not found")
    {
        ServiceName = serviceName;
    }

    public ServiceNotFoundException(string serviceName, Exception innerException)
        : base($"Service: {serviceName} not found", innerException)
    {
        ServiceName = serviceName;
    }
}

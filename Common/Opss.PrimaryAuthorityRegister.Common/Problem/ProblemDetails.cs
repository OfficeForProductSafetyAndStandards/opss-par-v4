using System.Net;
using System.Text.Json.Serialization;

namespace Opss.PrimaryAuthorityRegister.Common.Problem;

public class ProblemDetails
{
    public string? Type { get; set; }
    public HttpStatusCode Status { get; set; }
    public string Detail { get; set; }
    public string? StackTrace { get; set; }

    public ProblemDetails(HttpStatusCode status, Exception? exception, bool isDevelopment = true)
    {
        Status = status;
        Detail = exception?.Message ?? "An error occured. Please try again later.";
        StackTrace = isDevelopment ? exception?.StackTrace : string.Empty;
        Type = exception?.GetType().Name;
    }

    [JsonConstructor]
    public ProblemDetails(string? type, HttpStatusCode status, string detail, string? stackTrace)
    {
        Type = type;
        Status = status;
        Detail = detail;
        StackTrace = stackTrace;
    }
}

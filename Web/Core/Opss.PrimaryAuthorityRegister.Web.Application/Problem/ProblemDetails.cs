using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Problem;

public class ProblemDetails
{
    public string? Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string? TraceId { get; set; }
    public string Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public ProblemDetailsExceptionDetails[]? ExceptionDetails { get; set; }

    public ProblemDetails(int status, string detail)
    {
        Title = "Error";
        Status = status;
        Detail = detail;
    }

    public void ThrowException()
    {
        var exception = new ProblemDetailsException(this);
        throw exception;
    }
}

public class ProblemDetailsExceptionDetails
{
    public string? Message { get; set; }
    public string? Type { get; set; }
    public string? Raw { get; set; }
}

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException() { }

    public ProblemDetailsException(ProblemDetails problem)
        : base(problem?.ExceptionDetails?.FirstOrDefault()?.Raw)
    {
    }
}

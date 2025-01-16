namespace Opss.PrimaryAuthorityRegister.Web.Application.Problem;

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException() { }

    public ProblemDetailsException(ProblemDetails? problem)
        : base(problem?.ExceptionDetails?.FirstOrDefault()?.Raw)
    {
    }
}

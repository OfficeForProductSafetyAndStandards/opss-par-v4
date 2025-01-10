using System.Net;
using Opss.PrimaryAuthorityRegister.Web.Application.Problem;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Entities;

public class HttpObjectResponse
{
    public virtual HttpResponseMessage Message { get; private set; }

    public virtual ProblemDetails? Problem { get; private set; }

    public virtual HttpStatusCode StatusCode => Message.StatusCode;

    public virtual bool IsSuccessStatusCode => Message.IsSuccessStatusCode;

    public HttpObjectResponse(HttpResponseMessage httpResponseMessage, ProblemDetails? problem = null)
    {
        Message = httpResponseMessage;
        Problem = problem;
    }
}

public class HttpObjectResponse<T>
{
    public virtual HttpResponseMessage Message { get; private set; }

    public virtual T? Result { get; private set; }

    public virtual ProblemDetails? Problem { get; private set; }

    public virtual HttpStatusCode StatusCode => Message.StatusCode;

    public virtual bool IsSuccessStatusCode => Message.IsSuccessStatusCode;

    public HttpObjectResponse(HttpResponseMessage httpResponseMessage, T? result, ProblemDetails? problem = null)
    {
        Message = httpResponseMessage;
        Result = result;
        Problem = problem;
    }
}

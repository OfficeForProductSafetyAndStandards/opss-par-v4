using System.Net;
using Opss.PrimaryAuthorityRegister.Client.Problem;

namespace Opss.PrimaryAuthorityRegister.Client.Factories;

public class CustomHttpObjectResponse<T> : HttpObjectResponse<T>
{
    public override HttpResponseMessage Message => throw new NotImplementedException();
    public override ProblemDetails? Problem => throw new NotImplementedException();
    public override HttpStatusCode StatusCode => HttpStatusCode.OK;
    public override bool IsSuccessStatusCode => true;

    public CustomHttpObjectResponse(T result) : base(null, result)
    {
    }
}

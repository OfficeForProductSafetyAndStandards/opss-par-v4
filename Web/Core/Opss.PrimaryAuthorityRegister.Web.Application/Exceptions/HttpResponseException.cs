using System.Net;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Exceptions;

/// <summary>
/// An exception that allows for a given System.Net.Http.HttpResponseMessage to be
///     returned to the client.
/// </summary>
public class HttpResponseException : Exception
{
    /// <summary>
    /// Gets the HTTP response to return to the client.
    /// </summary>
    public HttpResponseMessage Response { get; private set; }

    /// <summary>
    /// Initializes a new instance of the System.Web.Http.HttpResponseException class.
    /// </summary>
    /// <param name="statusCode">The status code of the response.</param>
    /// <param name="content">The response content.</param>
    public HttpResponseException(HttpStatusCode statusCode, string? content)
        : this(new HttpResponseMessage(statusCode), content)
    {
    }

    /// <summary>
    /// Initializes a new instance of the System.Web.Http.HttpResponseException class.
    /// </summary>
    /// <param name="response">The HTTP response to return to the client.</param>
    /// <param name="content">The response content.</param>
    public HttpResponseException(HttpResponseMessage response, string? content)
        : base(content)
    {
        ArgumentNullException.ThrowIfNull(response);

        Response = response;
    }
}

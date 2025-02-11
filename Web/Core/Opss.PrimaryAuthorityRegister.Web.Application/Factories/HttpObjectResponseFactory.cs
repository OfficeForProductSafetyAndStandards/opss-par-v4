using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using Opss.PrimaryAuthorityRegister.Common.Problem;
using Opss.PrimaryAuthorityRegister.Web.Application.Exceptions;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Factories;

public static class HttpObjectResponseFactory
{
    private static readonly JsonSerializerOptions _serializationOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters =
                    {
                        new JsonStringEnumConverter()
                    }
    };

    public async static Task<HttpObjectResponse<T>> DetermineSuccess<T>(HttpResponseMessage httpResponseMessage) where T : class
    {
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var problem = TryRetrieveProblem<T>(httpResponseMessage).Problem;
            var exceptionMessage = problem?.Detail;
            if (!string.IsNullOrEmpty(problem?.StackTrace))
            {
                exceptionMessage += Environment.NewLine + problem.StackTrace;
            }
            throw new HttpResponseException(httpResponseMessage.StatusCode, exceptionMessage);
        }

        var result = default(T);
        var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(json))
        {
            if (typeof(T) == typeof(string))
            {
                result = (T)Convert.ChangeType(json, typeof(T), CultureInfo.InvariantCulture);
            }
            else
            {
                result = JsonSerializer.Deserialize<T>(json, _serializationOptions);
            }
        }

        return new HttpObjectResponse<T>(httpResponseMessage, result);
    }

    public async static Task<HttpObjectResponse> DetermineSuccess(HttpResponseMessage httpResponseMessage)
    {
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var problem = TryRetrieveProblem(httpResponseMessage);
            throw new HttpResponseException(httpResponseMessage.StatusCode, problem?.Problem?.Detail);
        }

        _ = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        return new HttpObjectResponse(httpResponseMessage);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "CA2201:Do not raise reserved exception types", 
        Justification = "Here a generic exception is thrown as we don't know what the problem is.")]
    private static HttpObjectResponse<T> TryRetrieveProblem<T>(HttpResponseMessage httpResponseMessage)
    {
        var unknownProblem = Problem<T>(new ProblemDetails(HttpStatusCode.InternalServerError, new ApplicationException("Unknown error occurred")));

        try
        {
            using var stream = httpResponseMessage.Content.ReadAsStream();
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            // If response body is not JSON, do not attempt to deserialize
            if (!(json?.Contains('{', StringComparison.InvariantCulture) ?? false)) return unknownProblem;

            var problem = JsonSerializer.Deserialize<ProblemDetails>(json, _serializationOptions);
            
            if (problem == null) return unknownProblem;
            
            return Problem<T>(problem);
        }
        catch(Exception ex)
        {
            return Problem<T>(new ProblemDetails(HttpStatusCode.InternalServerError, ex));
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Usage",
        "CA2201:Do not raise reserved exception types",
        Justification = "Here a generic exception is thrown as we don't know what the problem is.")]
    private static HttpObjectResponse TryRetrieveProblem(HttpResponseMessage httpResponseMessage)
    {
        var unknownProblem = Problem(new ProblemDetails(HttpStatusCode.InternalServerError, new ApplicationException("Unknown error occurred")));

        try
        {
            using var stream = httpResponseMessage.Content.ReadAsStream();
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            // If response body is not JSON, do not attempt to deserialize
            if (!(json?.Contains('{', StringComparison.InvariantCulture) ?? false)) return unknownProblem;

            var problem = JsonSerializer.Deserialize<ProblemDetails>(json, _serializationOptions);
            
            if (problem == null) return unknownProblem;

            return Problem(problem);
        }
        catch (Exception ex)
        {
            return Problem(new ProblemDetails(HttpStatusCode.InternalServerError, ex));
        }
    }

    public static HttpObjectResponse<T> Problem<T>(ProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);

        var json = JsonSerializer.Serialize(problem);
        var httpResponseMessage = new HttpResponseMessage(problem.Status)
        {
            StatusCode = problem.Status,
            Content = new StringContent(json)
        };

        return new HttpObjectResponse<T>(httpResponseMessage, default, problem);
    }

    public static HttpObjectResponse Problem(ProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);

        var json = JsonSerializer.Serialize(problem.Status);
        var httpResponseMessage = new HttpResponseMessage(problem.Status)
        {
            StatusCode = problem.Status,
            Content = new StringContent(json)
        };

        return new HttpObjectResponse(httpResponseMessage, problem);
    }
}

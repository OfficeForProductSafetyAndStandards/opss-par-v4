using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using Opss.PrimaryAuthorityRegister.Client.Exceptions;
using Opss.PrimaryAuthorityRegister.Client.Problem;

namespace Opss.PrimaryAuthorityRegister.Client.Factories;

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
            var problem = TryRetrieveProblem<T>(httpResponseMessage);
            throw new HttpResponseException(httpResponseMessage.StatusCode, problem.Problem.Detail);
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
            throw new HttpResponseException(httpResponseMessage.StatusCode, problem.Problem.Detail);
        }

        var json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        return new HttpObjectResponse(httpResponseMessage);
    }

    private static HttpObjectResponse<T> TryRetrieveProblem<T>(HttpResponseMessage httpResponseMessage)
    {
        var unknownProblem = Problem<T>(new ProblemDetails(StatusCodes.Status500InternalServerError, "Unknown error occurred"));

        try
        {
            using var stream = httpResponseMessage.Content.ReadAsStream();
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            // If response body is not JSON, do not attempt to deserialize
            if (!(json?.Contains('{', StringComparison.InvariantCulture) ?? false)) return unknownProblem;

            var problem = JsonSerializer.Deserialize<ProblemDetails>(json, _serializationOptions);
            return Problem<T>(problem);
        }
        catch
        {
            return unknownProblem;
        }
    }

    private static HttpObjectResponse TryRetrieveProblem(HttpResponseMessage httpResponseMessage)
    {
        var unknownProblem = Problem(new ProblemDetails(StatusCodes.Status500InternalServerError, "Unknown error occurred"));

        try
        {
            using var stream = httpResponseMessage.Content.ReadAsStream();
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();

            // If response body is not JSON, do not attempt to deserialize
            if (!(json?.Contains('{', StringComparison.InvariantCulture) ?? false)) return unknownProblem;

            var problem = JsonSerializer.Deserialize<ProblemDetails>(json, _serializationOptions);
            return Problem(problem);
        }
        catch
        {
            return unknownProblem;
        }
    }

    public static HttpObjectResponse<T> Problem<T>(ProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);

        var statusCode = (HttpStatusCode)problem.Status;

        var json = JsonSerializer.Serialize(problem);
        var httpResponseMessage = new HttpResponseMessage(statusCode)
        {
            StatusCode = statusCode,
            Content = new StringContent(json)
        };

        return new HttpObjectResponse<T>(httpResponseMessage, default, problem);
    }

    public static HttpObjectResponse Problem(ProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);

        var statusCode = (HttpStatusCode)problem.Status;

        var json = JsonSerializer.Serialize(problem);
        var httpResponseMessage = new HttpResponseMessage(statusCode)
        {
            StatusCode = statusCode,
            Content = new StringContent(json)
        };

        return new HttpObjectResponse(httpResponseMessage, problem);
    }
}

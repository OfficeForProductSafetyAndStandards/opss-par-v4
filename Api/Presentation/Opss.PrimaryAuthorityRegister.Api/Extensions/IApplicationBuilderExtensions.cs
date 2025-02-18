using Microsoft.AspNetCore.Diagnostics;
using Opss.PrimaryAuthorityRegister.Http.Problem;
using System.Net;
using System.Security;

namespace Opss.PrimaryAuthorityRegister.Api.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void AddExceptionHandlers(this IApplicationBuilder config, bool isDevelopment)
    {
        config.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is SecurityException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails(HttpStatusCode.Unauthorized, exception, isDevelopment)).ConfigureAwait(false);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails(HttpStatusCode.InternalServerError, exception, isDevelopment)).ConfigureAwait(false);
            }
        });
    }
}

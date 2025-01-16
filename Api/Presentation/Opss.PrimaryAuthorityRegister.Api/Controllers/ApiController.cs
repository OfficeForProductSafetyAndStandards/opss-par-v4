using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Common;
using System.Text.Json;

namespace Opss.PrimaryAuthorityRegister.Api.Controllers;

[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<object>> ExecutePost(string name, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) throw new InvalidDataException();

        object? request = await GetRequest(name).ConfigureAwait(false);

        if (request is BadRequestObjectResult) return request;

        var retVal = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

        return Responses.Created((Guid)retVal);
    }

    [HttpPut]
    public async Task<ActionResult> ExecutePut(string name, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) throw new InvalidDataException();

        object? request = await GetRequest(name).ConfigureAwait(false);

        _ = await _mediator.Send(request, cancellationToken).ConfigureAwait(false);

        return Responses.NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<object>> ExecuteQuery(string name, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) throw new InvalidDataException();

        object? request = await GetRequest(name).ConfigureAwait(false);

        return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<object?> GetRequest(string name)
    {
        Type? type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == name);

        if (type == null) return Responses.BadRequest($"Type: {name} could not be found");

        object? request;

        var body = await StreamToStringAsync(Request).ConfigureAwait(false);
        if (string.IsNullOrEmpty(body))
        {
            request = Activator.CreateInstance(type);
        }
        else
        {
            request = JsonSerializer.Deserialize(body, type);
        }

        return request;
    }

    private static async Task<string> StreamToStringAsync(HttpRequest request)
    {
        using (var sr = new StreamReader(request.Body))
        {
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;
using System.Text.Json;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Controllers;

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
        object? request = await GetRequest(name).ConfigureAwait(false);

        var retVal = await _mediator.Send(request);

        return Responses.Created((Guid)retVal);
    }

    [HttpPut]
    public async Task<ActionResult> ExecutePut(string name, CancellationToken cancellationToken)
    {
        object? request = await GetRequest(name).ConfigureAwait(false);

        await _mediator.Send(request).ConfigureAwait(false);

        return Responses.NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<object>> ExecuteQuery(string name, CancellationToken cancellationToken)
    {
        object? request = await GetRequest(name).ConfigureAwait(false);

        return await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<object?> GetRequest(string name)
    {
        if (!ModelState.IsValid) throw new InvalidDataException();

        Type? type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == name);

        if (type == null) return Responses.BadRequest($"Type: {name} could not be found");

        object? command;

        var body = await StreamToStringAsync(Request).ConfigureAwait(false);
        if (string.IsNullOrEmpty(body))
        {
            command = Activator.CreateInstance(type);
        }
        else
        {
            command = JsonSerializer.Deserialize(body, type);
        }

        return command;
    }

    private static async Task<string> StreamToStringAsync(HttpRequest request)
    {
        using (var sr = new StreamReader(request.Body))
        {
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}

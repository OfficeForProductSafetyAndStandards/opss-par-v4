using Microsoft.AspNetCore.Mvc;

namespace Opss.PrimaryAuthorityRegister.Cqrs;

public static class Responses
{
    public static NotFoundObjectResult NotFound(object? error = null) => new(error);
    public static BadRequestObjectResult BadRequest(object? error = null) => new(error);
    public static CreatedResult Created(Guid id) => Created(string.Empty, id);
    public static CreatedResult Created(IEnumerable<Guid> ids) => Created(string.Empty, ids);
    public static CreatedResult Created(string location, Guid id) => new(location, new CreatedResponse() { Id = id });
    public static CreatedResult Created(string location, IEnumerable<Guid> ids) => new(location, ids.Select(i => new CreatedResponse() { Id = i }));
    public static NoContentResult NoContent() => new NoContentResult();
    public static OkResult Ok() => new();
}

public class CreatedResponse
{
    public Guid Id { get; set; }
}

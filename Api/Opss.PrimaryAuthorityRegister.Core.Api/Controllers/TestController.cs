using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;
using Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;
using System.Data;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class TestController : ControllerBase
{
    private readonly IMediator _mediator;

    public TestController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("data")]
    public async Task<ActionResult<TestDataDto>> Get(CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetTestDataQuery(), cancellationToken).ConfigureAwait(false);
    }

    [HttpPost("data")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTestDataCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
    }

    [HttpPut("data")]
    public async Task<ActionResult> Update([FromBody] UpdateTestDataCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
    }
}

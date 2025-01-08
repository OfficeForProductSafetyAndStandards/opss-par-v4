using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;
using Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers;

internal class GetTestDataQueryHandler : BaseRequestHandler<GetTestDataQuery, ActionResult<TestDataDto>>
{
    public GetTestDataQueryHandler(ILogger<GetTestDataQueryHandler> logger) : base(logger)
    {
    }

    public override async Task<ActionResult<TestDataDto>> Handle(GetTestDataQuery request, CancellationToken cancellationToken)
    {
        return new TestDataDto();
    }
}

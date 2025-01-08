using MediatR;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;
using Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers;

internal class GetTestDataQueryHandler : IRequestHandler<GetTestDataQuery, TestDataDto>
{
    public GetTestDataQueryHandler()
    {
    }

    public async Task<TestDataDto> Handle(GetTestDataQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await Task.FromResult(new TestDataDto()).ConfigureAwait(false);
    }
}
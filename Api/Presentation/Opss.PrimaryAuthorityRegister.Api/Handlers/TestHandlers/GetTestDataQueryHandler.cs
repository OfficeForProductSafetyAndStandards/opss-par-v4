using MediatR;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Api.Handlers.TestHandlers;

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
using MediatR;
using Opss.PrimaryAuthorityRegister.Common.Requests.Common.Authority.Queries;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Authority.Queries;

public class GetMyOfferedRegulatoryFunctionsQueryHandler : IRequestHandler<GetMyOfferedRegulatoryFunctionsQuery, List<string>>
{
    public Task<List<string>> Handle(GetMyOfferedRegulatoryFunctionsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<string>
        {
            "Environmental Health",
            "Trading Standards"
        });
    }
}

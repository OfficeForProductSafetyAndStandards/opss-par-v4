using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;

public class GetTestDataQueryHandler : IRequestHandler<GetTestDataQuery, TestDataDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTestDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TestDataDto> Handle(GetTestDataQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var data = await _unitOfWork.Repository<TestData>().GetByIdAsync(request.Id).ConfigureAwait(false);

        if (data == null)
        {
            throw new InvalidOperationException($"No data found with Id {request.Id}");
        }

        return new TestDataDto(data.Id, data.Data);
    }
}
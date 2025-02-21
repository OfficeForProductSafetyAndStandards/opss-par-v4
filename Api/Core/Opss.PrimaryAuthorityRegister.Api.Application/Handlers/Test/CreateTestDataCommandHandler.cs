using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;

public class CreateTestDataCommandHandler : IRequestHandler<CreateTestDataCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTestDataCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var data = await _unitOfWork.Repository<TestData>().AddAsync(new TestData(request.OwnerId, request.Data)).ConfigureAwait(false);

        return data.Id;
    }
}

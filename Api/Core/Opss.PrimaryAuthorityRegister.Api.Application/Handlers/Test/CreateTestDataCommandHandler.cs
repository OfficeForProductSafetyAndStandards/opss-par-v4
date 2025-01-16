using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;

public class CreateTestDataCommandHandler : IRequestHandler<CreateTestDataCommand, Guid>
{
    private readonly IUnitOfWork unitOfWork;

    public CreateTestDataCommandHandler(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var data = await unitOfWork.Repository<TestData>().AddAsync(new TestData(request.Data)).ConfigureAwait(false);

        return data.Id;
    }
}

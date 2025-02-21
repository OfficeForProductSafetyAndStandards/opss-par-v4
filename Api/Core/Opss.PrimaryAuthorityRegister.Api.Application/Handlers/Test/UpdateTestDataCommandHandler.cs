using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;

public class UpdateTestDataCommandHandler : IRequestHandler<UpdateTestDataCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTestDataCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateTestDataCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var data = await _unitOfWork.Repository<TestData>().GetByIdAsync(request.Id).ConfigureAwait(false);

        if(data == null)
        {
            throw new InvalidOperationException($"No data found with Id {request.Id}");
        }

        data.Data = request.Data;

        await _unitOfWork.Repository<TestData>().UpdateAsync(data).ConfigureAwait(false);
    }
}

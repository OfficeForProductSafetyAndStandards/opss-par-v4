using MediatR;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Handlers.TestHandlers;

internal class CreateTestDataCommandHandler : IRequestHandler<CreateTestDataCommand, Guid>
{
    public CreateTestDataCommandHandler()
    {
    }

    public async Task<Guid> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var guid = Guid.NewGuid();

        return await Task.FromResult(guid).ConfigureAwait(false);
    }
}

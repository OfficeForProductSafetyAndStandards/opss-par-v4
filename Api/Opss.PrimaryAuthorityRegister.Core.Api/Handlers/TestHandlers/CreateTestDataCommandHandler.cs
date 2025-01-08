using MediatR;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers;

internal class CreateTestDataCommandHandler : IRequestHandler<CreateTestDataCommand, Guid>
{
    public CreateTestDataCommandHandler()
    {
    }

    public async Task<Guid> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var guid = Guid.NewGuid();

        return guid;
    }
}

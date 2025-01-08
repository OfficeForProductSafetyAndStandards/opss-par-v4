using MediatR;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers
{
    public class UpdateTestDataCommandHandler : IRequestHandler<UpdateTestDataCommand>
    {
        public UpdateTestDataCommandHandler(ILogger<UpdateTestDataCommandHandler> logger)
        {
        }

        public async Task Handle(UpdateTestDataCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            // do nothing
        }
    }
}

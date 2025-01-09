using MediatR;
using Opss.PrimaryAuthorityRegister.Common.Services.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Handlers.TestHandlers
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
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}

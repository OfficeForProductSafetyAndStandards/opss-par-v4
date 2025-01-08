using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers
{
    public class UpdateTestDataCommandHandler : BaseRequestHandler<UpdateTestDataCommand, ActionResult>
    {
        public UpdateTestDataCommandHandler(ILogger<UpdateTestDataCommandHandler> logger) : base(logger)
        {
        }

        public override async Task<ActionResult> Handle(UpdateTestDataCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await Task.FromResult(Responses.Ok()).ConfigureAwait(false);
        }
    }
}

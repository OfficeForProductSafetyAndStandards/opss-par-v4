using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;
using Opss.PrimaryAuthorityRegister.Core.Common.Mediator;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers
{
    public class CreateTestDataCommandHandler : BaseRequestHandler<CreateTestDataCommand, ActionResult<Guid>>
    {
        public CreateTestDataCommandHandler(ILogger<CreateTestDataCommandHandler> logger) : base(logger)
        {
        }

        public override async Task<ActionResult<Guid>> Handle(CreateTestDataCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await Task.FromResult(Responses.Created(Guid.NewGuid())).ConfigureAwait(false);
        }
    }
}

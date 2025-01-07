using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Core.Common;
using Opss.PrimaryAuthorityRegister.Core.Common.Commands;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Handlers.TestHandlers
{
    public class TestDataCommandHandler : BaseRequestHandler<TestDataCommand, ActionResult>
    {
        public TestDataCommandHandler(ILogger<TestDataCommandHandler> logger) : base(logger)
        {
        }

        public override async Task<ActionResult> Handle(TestDataCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await Task.FromResult(Responses.Ok()).ConfigureAwait(false);
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;

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

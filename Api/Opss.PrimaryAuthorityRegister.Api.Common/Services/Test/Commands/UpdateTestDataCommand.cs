using Opss.PrimaryAuthorityRegister.Api.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Api.Common.Services.Test.Commands;

public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }

    public UpdateTestDataCommand(Guid id)
    {
        Id = id;
    }
}

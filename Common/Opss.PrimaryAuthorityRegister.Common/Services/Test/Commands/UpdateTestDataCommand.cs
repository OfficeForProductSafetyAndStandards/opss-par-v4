using Opss.PrimaryAuthorityRegister.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Common.Services.Test.Commands;

public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }

    public UpdateTestDataCommand(Guid id)
    {
        Id = id;
    }
}

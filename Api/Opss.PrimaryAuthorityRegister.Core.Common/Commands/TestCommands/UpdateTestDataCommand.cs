using Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;

public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }

    public UpdateTestDataCommand(Guid id)
    {
        Id = id;
    }
}

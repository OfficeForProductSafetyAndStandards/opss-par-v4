using Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;

public class CreateTestDataCommand : ICommand<Guid>
{
    public Guid Id { get; set; }

    public CreateTestDataCommand(Guid id)
    {
        Id = id;
    }
}
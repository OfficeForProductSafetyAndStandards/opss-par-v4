using Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Commands.TestCommands;

public class CreateTestDataCommand : ICommand<Guid>
{
    public CreateTestDataCommand(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
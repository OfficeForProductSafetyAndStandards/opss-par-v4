using Opss.PrimaryAuthorityRegister.Common.Cqrs;

namespace Opss.PrimaryAuthorityRegister.Common.Services.Test.Commands;

public class CreateTestDataCommand : ICommand<Guid>
{
    public CreateTestDataCommand(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

public class CreateTestDataCommand : ICommand<Guid>
{
    public CreateTestDataCommand(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
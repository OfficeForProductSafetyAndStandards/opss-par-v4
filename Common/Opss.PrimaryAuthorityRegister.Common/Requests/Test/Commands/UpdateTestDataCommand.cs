using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }

    public UpdateTestDataCommand(Guid id)
    {
        Id = id;
    }
}

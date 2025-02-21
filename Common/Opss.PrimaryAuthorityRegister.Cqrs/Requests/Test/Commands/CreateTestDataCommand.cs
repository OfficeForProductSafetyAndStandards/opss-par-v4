using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

[PermissionFor("Create", "Owner/{OwnerId}")]
public class CreateTestDataCommand : ICommand<Guid>
{
    public Guid OwnerId { get; }
    public string Data { get; }

    public CreateTestDataCommand(Guid ownerId, string data)
    {
        OwnerId = ownerId;
        Data = data;
    }
}
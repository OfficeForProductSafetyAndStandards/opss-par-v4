using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

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
using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

[PermissionFor("Create", "TestData/*", Group = "OPSS")]
public class CreateTestDataCommand : ICommand<Guid>
{
    public string Data { get; }
    
    public CreateTestDataCommand(string data)
    {
        Data = data;
    }
}
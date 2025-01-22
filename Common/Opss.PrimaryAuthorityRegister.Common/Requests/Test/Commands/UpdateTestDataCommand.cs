using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

/// <summary>
/// Permission For Attribute: User must have write permissions on TestData with the given Id and belong to the OPSS group.
/// </summary>
[PermissionFor("Write", "TestData/{Id}", Group = "OPSS")]
public class UpdateTestDataCommand : ICommand
{
    public Guid Id { get; set; }
    public string Data { get; set; }

    public UpdateTestDataCommand(Guid id, string data)
    {
        Id = id;
        Data = data;
    }
}

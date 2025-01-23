using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

/// <summary>
/// Permission For Attribute: User must have write permissions on TestData with the given Id and belong to the OPSS group.
/// </summary>
[PermissionFor("Write", "TestData/*")]
public class UpdateTestDataCommand : ICommand
{
    public Guid OwnerId { get; }
    public Guid Id { get; }
    public string Data { get; }

    public UpdateTestDataCommand(Guid ownerId, Guid id, string data)
    {
        OwnerId = ownerId;
        Id = id;
        Data = data;
    }
}

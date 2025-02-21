using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

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

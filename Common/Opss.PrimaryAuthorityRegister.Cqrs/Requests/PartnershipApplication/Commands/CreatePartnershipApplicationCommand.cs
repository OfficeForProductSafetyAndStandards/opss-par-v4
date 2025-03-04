using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;

public class CreatePartnershipApplicationCommand : ICommand<Guid>
{
    public CreatePartnershipApplicationCommand(string partnershipType)
    { }

    public string PartnershipType { get; }
}

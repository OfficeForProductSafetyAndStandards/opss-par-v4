namespace Opss.PrimaryAuthorityRegister.Authentication.Entities;

public class AuthenticatedUserIdentity : IAuthenticatedUserIdentity<AuthenticatedUserRole>
{
    public Guid Id { get; private set; }
    public string EmailAddress { get; private set; }
    public IReadOnlyCollection<AuthenticatedUserRole> Roles => _roles;

    private readonly List<AuthenticatedUserRole> _roles;

    public AuthenticatedUserIdentity(Guid id, string emailAddress, AuthenticatedUserRole[] roles)
    {
        Id = id;
        EmailAddress = emailAddress;
        _roles = roles.ToList();
    }
}

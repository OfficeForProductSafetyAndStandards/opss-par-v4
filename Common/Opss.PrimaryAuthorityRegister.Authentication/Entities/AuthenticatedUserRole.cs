namespace Opss.PrimaryAuthorityRegister.Authentication.Entities;

public class AuthenticatedUserRole : IAuthenticatedUserRole
{
    public string Name { get; private set; }

    public AuthenticatedUserRole(string name)
    {
        Name = name;
    }
}
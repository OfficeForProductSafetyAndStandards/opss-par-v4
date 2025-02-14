namespace Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authentication;

public interface ITokenService
{
    Task ValidateTokenAsync(string idToken, CancellationToken cancellationToken);
    string GenerateJwtToken(string email);
}

﻿namespace Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

public interface ITokenService
{
    Task ValidateTokenAsync(string providerKey, string idToken, CancellationToken cancellationToken);
    string GenerateJwt(string email);
}

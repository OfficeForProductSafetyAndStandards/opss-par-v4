using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Opss.PrimaryAuthorityRegister.Web.Authentication;

public class OpenIdConnectEvents : Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
{
    private readonly OneLoginAuthConfig _oneLoginAuthConfig;
    private readonly JwtAuthConfig _chmmJwtAuthConfig;

    private readonly CookieOptions _cookieOptions;

    public OpenIdConnectEvents(IOptions<OneLoginAuthConfig> oneLoginAuthConfig, IOptions<JwtAuthConfig> chmmJwtAuthConfig)
    {
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);
        ArgumentNullException.ThrowIfNull(chmmJwtAuthConfig);

        _oneLoginAuthConfig = oneLoginAuthConfig.Value;
        _chmmJwtAuthConfig = chmmJwtAuthConfig.Value;
        _cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(_oneLoginAuthConfig.CookieMaxAge)
        };
    }

    private JwtSecurityToken? GetJwtSecurityToken(SigningCredentials signingCredentials)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return new JwtSecurityToken(
            audience: $"{_oneLoginAuthConfig.Authority}/token",
            issuer: _oneLoginAuthConfig.ClientId,
            claims: new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _oneLoginAuthConfig.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.Integer64)
            },
            expires: DateTime.UtcNow.AddMinutes(3),
            signingCredentials: signingCredentials
        );
    }

    private IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_chmmJwtAuthConfig.SecurityKey);
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = _chmmJwtAuthConfig.Issuer,
            ValidAudience = _chmmJwtAuthConfig.Audience,
            ClockSkew = TimeSpan.FromSeconds(_chmmJwtAuthConfig.ClockSkewSeconds)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        return claimsPrincipal.Claims;
    }

    public override async Task RedirectToIdentityProvider(RedirectContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await base.RedirectToIdentityProvider(context).ConfigureAwait(false);
        context.ProtocolMessage.SetParameter("vtr", "[Cl]");
    }

    public override async Task TicketReceived(TicketReceivedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Properties is AuthenticationProperties properties)
        {
            var tokens = properties.GetTokens().ToList();

            var idToken = tokens.FirstOrDefault(t => t.Name == "id_token")?.Value;
            var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token")?.Value;

            var response = await _identityService.GetJwtToken(new GetJwtTokenQuery(idToken, accessToken)).ConfigureAwait(false);

            if (response.IsSuccessStatusCode && response.Result is not null)
            {
                var authToken = response.Result;
                var claims = GetClaimsFromToken(authToken);
                var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);

                if (context.Principal is ClaimsPrincipal principal)
                {
                    principal.AddIdentity(identity);
                    tokens.Add(new AuthenticationToken()
                    {
                        Name = AuthenticationConstants.TokenName,
                        Value = authToken
                    });
                }
                properties.StoreTokens(tokens);
            }
        }
    }

    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(_oneLoginAuthConfig.RsaPrivateKey);
        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();

        var token = GetJwtSecurityToken(signingCredentials);
        var jwt = tokenHandler.WriteToken(token);

        if (context.TokenEndpointRequest is OpenIdConnectMessage tokenEndpointRequest)
        {
            tokenEndpointRequest.ClientAssertionType = ClientAssertionTypes.JwtBearer;
            tokenEndpointRequest.ClientAssertion = jwt;
        }

        if (context.Request.Query.TryGetValue("state", out var state))
        {
            context.Response.Cookies.Append(OpenIdConnectCookies.OneLoginState, state, _cookieOptions);
        }

        return Task.CompletedTask;
    }

    public override async Task RedirectToIdentityProviderForSignOut(RedirectContext context)
    {
        context.HttpContext.Response.Cookies.Delete(OpenIdConnectCookies.OneLoginState);
        context.HttpContext.Response.Cookies.Delete(OpenIdConnectCookies.ParToken);

        var props = context.ProtocolMessage;
        props.PostLogoutRedirectUri = _oneLoginAuthConfig.PostLogoutRedirectUri;

        var idToken = await context.HttpContext.GetTokenAsync("id_token");
        props.IdTokenHint = idToken;

        if (context.Request.Cookies.TryGetValue(OpenIdConnectCookies.OneLoginState, out var state))
        {
            props.State = state;
        }
    }
}


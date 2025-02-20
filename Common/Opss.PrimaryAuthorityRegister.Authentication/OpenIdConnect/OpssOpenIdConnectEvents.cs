using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Common.ExtensionMethods;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Services;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public class OpssOpenIdConnectEvents : OpenIdConnectEvents
{
    private readonly OpenIdConnectAuthConfig _oidcAuthConfig;
    private readonly JwtAuthConfig _jwtAuthConfig;

    private readonly CookieOptions _cookieOptions;
    private readonly IHttpService _httpService;

    public OpssOpenIdConnectEvents(OpenIdConnectAuthConfig oidcAuthConfig, JwtAuthConfig jwtAuthConfig, IHttpService httpService)
    {
        ArgumentNullException.ThrowIfNull(oidcAuthConfig);
        ArgumentNullException.ThrowIfNull(jwtAuthConfig);

        _oidcAuthConfig = oidcAuthConfig;
        _jwtAuthConfig = jwtAuthConfig;
        _cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(_oidcAuthConfig.CookieMaxAge)
        };
        _httpService = httpService;
    }

    private JwtSecurityToken? GetJwtSecurityToken(SigningCredentials signingCredentials)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        return new JwtSecurityToken(
            audience: _oidcAuthConfig.AuthorityUri.AppendPath(_oidcAuthConfig.AccessTokenPath).ToString(),
            issuer: _oidcAuthConfig.ClientId,
            claims: new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _oidcAuthConfig.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64)
            },
            expires: DateTime.UtcNow.AddMinutes(3),
            signingCredentials: signingCredentials
        );
    }

    private IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var key = Encoding.ASCII.GetBytes(_jwtAuthConfig.SecurityKey);
        var validationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = _jwtAuthConfig.IssuerUri.ToString(),
            ValidAudience = _jwtAuthConfig.AudienceUri.ToString(),
            ClockSkew = TimeSpan.FromSeconds(_jwtAuthConfig.ClockSkewSeconds)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);

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

        if (context.Properties is not AuthenticationProperties properties)
            return;

        var tokens = properties.GetTokens().ToList();

        var idToken = tokens.First(t => t.Name == "id_token").Value;
        var accessToken = tokens.First(t => t.Name == "access_token").Value;
        var jwtQuery = new GetJwtQuery(_oidcAuthConfig.ProviderKey, idToken, accessToken);

        var response = await _httpService.GetAsync<GetJwtQuery, string>(jwtQuery).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode || response.Result is null)
            return;

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

    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Request.Query.TryGetValue("state", out var state))
        {
            context.Response.Cookies.Append(OpenIdConnectCookies.AuthState, state!, _cookieOptions);
        }

        if (string.IsNullOrEmpty(_oidcAuthConfig.RsaPrivateKey))
            return Task.CompletedTask;

        using var rsa = RSA.Create();
        rsa.ImportFromPem(_oidcAuthConfig.RsaPrivateKey);
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

        return Task.CompletedTask;
    }

    public override async Task RedirectToIdentityProviderForSignOut(RedirectContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.HttpContext.Response.Cookies.Delete(OpenIdConnectCookies.AuthState);
        context.HttpContext.Response.Cookies.Delete(OpenIdConnectCookies.ParToken);

        var props = context.ProtocolMessage;
        props.PostLogoutRedirectUri = _oidcAuthConfig.PostLogoutRedirectUri.ToString();

        var idToken = await context.HttpContext.GetTokenAsync("id_token").ConfigureAwait(false);
        props.IdTokenHint = idToken;

        if (context.Request.Cookies.TryGetValue(OpenIdConnectCookies.AuthState, out var state))
        {
            props.State = state;
        }
    }
}


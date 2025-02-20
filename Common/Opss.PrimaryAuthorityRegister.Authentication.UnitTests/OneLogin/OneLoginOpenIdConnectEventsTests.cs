using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Authentication.OneLogin;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OneLogin;

public class OneLoginOpenIdConnectEventsTests
{
    private readonly Mock<IHttpService> _mockHttpService;
    private readonly OpssOpenIdConnectEvents _events;
    private readonly OpenIdConnectAuthConfig _mockOneLoginConfig;
    private readonly JwtAuthConfig _mockJwtAuthConfig;

    public OneLoginOpenIdConnectEventsTests()
    {
        _mockHttpService = new Mock<IHttpService>();
        _mockOneLoginConfig = new OpenIdConnectAuthConfig
        {
            AuthorityUri = new Uri("https://example.com"),
            IssuerUri = new Uri("https://example.com"),
            ClientId = "client-id",
            CookieMaxAge = 30,
            // This RSA key was generaated specifically for this test file and is used nowhere else in the system.
            RsaPrivateKey = StaticTestRsaKey.Value,
            PostLogoutRedirectUri = new Uri("https://localhost"),
            ClockSkewSeconds = 60,
            WellKnownPath = "/.well-known/openid-configuration",
            UserInfoPath = "/userinfo",
            AccessTokenPath = "/accesstoken",
            CallbackPath = "/onelogin-signin-oidc"
        };
        _mockJwtAuthConfig = new JwtAuthConfig
        {
            SecurityKey = "supersecretkeysupersecretkeysupersecretkey",
            IssuerUri = new Uri("https://localhost/"),
            AudienceUri = new Uri("https://localhost/"),
            ClockSkewSeconds = 300
        };

        _events = new OpssOpenIdConnectEvents(_mockOneLoginConfig, _mockJwtAuthConfig, _mockHttpService.Object);
    }

    [Fact]
    public async Task RedirectToIdentityProvider_SetsVtrParameter()
    {
        var context = new RedirectContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new OpenIdConnectOptions(),
            new AuthenticationProperties())
        {
            ProtocolMessage = new OpenIdConnectMessage()
        };

        await _events.RedirectToIdentityProvider(context);

        Assert.Equal("[Cl]", context.ProtocolMessage.GetParameter("vtr"));
    }

    [Fact]
    public async Task TicketReceived_AddsCustomTokenAndClaims()
    {
        string jwt = generateJwt();

        var authProperties = new AuthenticationProperties();
        authProperties.StoreTokens(new List<AuthenticationToken>
        {
            new AuthenticationToken { Name = "id_token", Value = "mock_id_token" },
            new AuthenticationToken { Name = "access_token", Value = "mock_access_token" }
        });
        var context = new TicketReceivedContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new RemoteAuthenticationOptions(),
            new AuthenticationTicket(new ClaimsPrincipal(), authProperties, ""));

        _mockHttpService
            .Setup(h => h.GetAsync<GetJwtTokenQuery, string>(It.IsAny<GetJwtTokenQuery>()))
            .ReturnsAsync(new HttpObjectResponse<string>(new HttpResponseMessage(System.Net.HttpStatusCode.OK), jwt));

        await _events.TicketReceived(context);

        var tokens = context.Properties.GetTokens();
        Assert.Contains(tokens, t => t.Name == AuthenticationConstants.TokenName && t.Value == jwt);
    }

    private string generateJwt()
    {
        var key = Encoding.UTF8.GetBytes(_mockJwtAuthConfig.SecurityKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _mockJwtAuthConfig.IssuerUri.ToString(),
            audience: _mockJwtAuthConfig.AudienceUri.ToString(),
            claims: Array.Empty<Claim>(),
            expires: DateTime.UtcNow.AddMinutes(_mockJwtAuthConfig.MinutesUntilExpiration),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }

    [Fact]
    public async Task RedirectToIdentityProviderForSignOut_DeletesCookies()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var authenticationServiceMock = new Mock<IAuthenticationService>();
        var authResult = AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(), null));

        authResult.Properties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "id_token", Value = "accessTokenValue" }
        });

        authenticationServiceMock
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), null))
            .ReturnsAsync(authResult);

        serviceProvider.Setup(_ => _.GetService(typeof(IAuthenticationService))).Returns(authenticationServiceMock.Object);


        var context = new RedirectContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new OpenIdConnectOptions(),
            new AuthenticationProperties())
        {
            ProtocolMessage = new OpenIdConnectMessage()
        };
        context.HttpContext.RequestServices = serviceProvider.Object;


        context.HttpContext.Response.Cookies.Append(OpenIdConnectCookies.AuthState, "state_value");
        context.HttpContext.Response.Cookies.Append(OpenIdConnectCookies.ParToken, "par_value");

        await _events.RedirectToIdentityProviderForSignOut(context);

        Assert.Null(context.HttpContext.Request.Cookies[OpenIdConnectCookies.AuthState]);
        Assert.Null(context.HttpContext.Request.Cookies[OpenIdConnectCookies.ParToken]);
    }

    [Fact]
    public async Task AuthorizationCodeReceived_SetsClientAssertion()
    {
        var context = new AuthorizationCodeReceivedContext(
            new DefaultHttpContext(),
            new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
            new OpenIdConnectOptions(),
            new AuthenticationProperties())
        {
            TokenEndpointRequest = new OpenIdConnectMessage()
        };
        var queryCollection = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
    {
        { "state", "test-state-value" }
    });
        context.HttpContext.Request.Query = queryCollection;
        

        await _events.AuthorizationCodeReceived(context);

        Assert.Equal(ClientAssertionTypes.JwtBearer, context.TokenEndpointRequest.ClientAssertionType);
        Assert.NotNull(context.TokenEndpointRequest.ClientAssertion);
        var setCookieHeaders = context.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains($"{OpenIdConnectCookies.AuthState}=test-state-value", setCookieHeaders);
    }
}

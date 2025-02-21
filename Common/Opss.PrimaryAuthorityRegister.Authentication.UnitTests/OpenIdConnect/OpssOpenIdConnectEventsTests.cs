using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.UnitTests.OpenIdConnect;

public class OpssOpenIdConnectEventsTests
{
    private readonly Mock<ICqrsService> _mockCqrsService;
    private readonly OpssOpenIdConnectEvents _events;

    public OpssOpenIdConnectEventsTests()
    {
        _mockCqrsService = new Mock<ICqrsService>();

        _events = new OpssOpenIdConnectEvents(AuthenticationTestHelpers.ProviderAuthConfig, AuthenticationTestHelpers.JwtConfig, _mockCqrsService.Object);
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

        _mockCqrsService
            .Setup(h => h.GetAsync<GetJwtQuery, string>(It.IsAny<GetJwtQuery>()))
            .ReturnsAsync(new HttpObjectResponse<string>(new HttpResponseMessage(System.Net.HttpStatusCode.OK), jwt));

        await _events.TicketReceived(context);

        var tokens = context.Properties.GetTokens();
        Assert.Contains(tokens, t => t.Name == AuthenticationConstants.TokenName && t.Value == jwt);
    }

    [Fact]
    public async Task WhenGetJwtFailes_TicketReceived_DoesNotAddsCustomTokenAndClaims()
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

        _mockCqrsService
            .Setup(h => h.GetAsync<GetJwtQuery, string>(It.IsAny<GetJwtQuery>()))
            .ReturnsAsync(new HttpObjectResponse<string>(
                new HttpResponseMessage(HttpStatusCode.BadRequest),
                "Something",
                new Http.Problem.ProblemDetails(HttpStatusCode.BadRequest, new Exception("Bad Request"))));

        await _events.TicketReceived(context);

        var tokens = context.Properties.GetTokens();
        Assert.DoesNotContain(tokens, t => t.Name == AuthenticationConstants.TokenName && t.Value == jwt);
    }

    [Fact]
    public async Task WhenGetJwtReturnsNull_TicketReceived_DoesNotAddsCustomTokenAndClaims()
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

        _mockCqrsService
            .Setup(h => h.GetAsync<GetJwtQuery, string>(It.IsAny<GetJwtQuery>()))
            .ReturnsAsync(new HttpObjectResponse<string>(
                new HttpResponseMessage(HttpStatusCode.BadRequest),
                null,
                null));

        await _events.TicketReceived(context);

        var tokens = context.Properties.GetTokens();
        Assert.DoesNotContain(tokens, t => t.Name == AuthenticationConstants.TokenName && t.Value == jwt);
    }

    private string generateJwt()
    {
        var key = Encoding.UTF8.GetBytes(AuthenticationTestHelpers.JwtConfig.SecurityKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: AuthenticationTestHelpers.JwtConfig.IssuerUri.ToString(),
            audience: AuthenticationTestHelpers.JwtConfig.AudienceUri.ToString(),
            claims: Array.Empty<Claim>(),
            expires: DateTime.UtcNow.AddMinutes(AuthenticationTestHelpers.JwtConfig.MinutesUntilExpiration),
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

    [Fact]
    public async Task WhenNoRSAKey_AuthorizationCodeReceived_SetsClientAssertion()
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

        var config = AuthenticationTestHelpers.ProviderAuthConfig;
        config.RsaPrivateKey = null;
        var events = new OpssOpenIdConnectEvents(config, AuthenticationTestHelpers.JwtConfig, _mockCqrsService.Object);
        await events.AuthorizationCodeReceived(context);

        Assert.Null(context.TokenEndpointRequest.ClientAssertion);
        var setCookieHeaders = context.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains($"{OpenIdConnectCookies.AuthState}=test-state-value", setCookieHeaders);
    }
}

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
    private readonly OneLoginOpenIdConnectEvents _events;
    private readonly OpenIdConnectAuthConfig _mockOneLoginConfig;
    private readonly JwtAuthConfig _mockJwtAuthConfig;

    public OneLoginOpenIdConnectEventsTests()
    {
        _mockHttpService = new Mock<IHttpService>();
        _mockOneLoginConfig = new OpenIdConnectAuthConfig
        {
            Authority = "https://example.com",
            ClientId = "client-id",
            CookieMaxAge = 30,
            // This RSA key was generaated specifically for this test file and is used nowhere else in the system.
            RsaPrivateKey = "-----BEGIN PRIVATE KEY-----MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDCjsiQBY+xZVpihpeJECwQEfar9bpmX5jg7t3EOJZ6y8b4zUsmMbPiJulDtoDuNjoP49j21eEFPeJJSIckCql1Wt1eQWc56vx9iHnZS+f++XxQkOxMyWz+o5bz+P8Rp/KZ90JuYS0VSgs5YBYYBBPvqRUkp8sdfiMlMwrhrpNZdNiJw/inclRwbvZ4A8HF4eAFecY3Xz1X/9SmhMbfqqG2W0keO3/TbUr/jTaFA3rM+no3S7q6xJmrxGYFfsvalgDRxudKMMzSw1RVo2Uli//mQVf0pPZOLAF+7paq/vza+PCl4dGp0MhKz0WT7zfkKUswgDTohHC+VTMcAZnHFo2pAgMBAAECggEAH7sKwdZuW4fEqHn7/+onzY0odl4ywtyHgfPjLkvuWuOeNVjCD14e0Nz4wUbkGzWz3YCTE5rJQsIXhE00YS/t+EpT/TncfIkzGcQm30YudZq56CfiqhlV0efbSDoNW5NREUROzNLDeAvl7bsaj1sm5zjjzmEhUtOOJtR+y1TeCkInrT4enr3OZegF714EcywEDhzH+5BnULGggP97c1v/CujqZ865AN2RN9EkNWOEHMbywetP8V8QyzOPk5dBuB4fkp8B0uUQwTaQCr/OAtRBX+7E05kLfp5jA5jCBoOlw49phY3L0azdXETQJnMmACJ86NhYrjeMpPC/iImjHl9CawKBgQD0xY8/tRr57YNIrMpZ8T86RRRFQGWXcGZMOuvaVq4QM637EId3kUeMyTaJi/+/yEjIFbjJNPhCYtltxTU4bgPNH0ORZsDCRyl++0sQpj2xggVFBPPQe36kfQ6yTLktwS33jDEpg3eWHoRePzKDOZ7ThlU7jON26lJcLspI9qeWDwKBgQDLe4snH9ygG5wnSayvnn63L04valZ5E/6Hh/e2D/m+mbTzHklGexz2hhYy9Ci8GeCXTBjnkQ4SCUJ3zGSDZ2bGNihb6hlDYuN9D+i5aVky84GGjQRc+mcZlfHpiP9BNc+wfmiFAJ8JQvVBw3qysgadw0Mp9wRpZF2/ATYH0siYxwKBgDxmc87YtetufLL2UIiZS2zplvLvzSHtjpDJCWI7eYBuAESv833Bz9Ih5N9UOKvulGcrVQnxlEFtexHnVBa0ryNyz42VuYM3ZDn9cKyPGTIwT3SUoEWV885LPdEptZhgzyMC6S7BTkUxCqDnH2PaWCMCRw4G2iqB8AjnUutmUjxpAoGAZ+9T1YklxTY1HbA5H38ilGj6U6fKQZAf65Rcx8cDNwMF9UScAv8xfQ5iWmZyRBonqMA63FUwTbAjHlPtZ10ylr3lAYXin5PsRN4SblpMMIVGvLZc6y0P6na3mSTb1LAqxKjctErr8OwdoBi8HHhofr7VGKNOwpJ8e+qfcYGN6tMCgYAXVRBNUKP9TVvSn79cb4lmeEBwrsHTN/F+3IPHOxevw62/1EaagvoCHlURo9TaHy4+i3v9kIWakbf8NrQG7Z5kTZi7GOiEwvhWHOqld8YPI8TU2I6jb+y+yJyDlAOuJJH03F+TRLNG0P7VBA+1UUkwaYkjlFZ8+MH/ZizvaOS23w==-----END PRIVATE KEY-----",
            PostLogoutRedirectUri = new Uri("https://localhost"),
            ClockSkewSeconds = 60
        };
        _mockJwtAuthConfig = new JwtAuthConfig
        {
            SecurityKey = "supersecretkeysupersecretkeysupersecretkey",
            Issuer = "issuer",
            Audience = "audience",
            ClockSkewSeconds = 300
        };

        var oneLoginOptions = Options.Create(_mockOneLoginConfig);
        var jwtOptions = Options.Create(_mockJwtAuthConfig);

        _events = new OneLoginOpenIdConnectEvents(oneLoginOptions, jwtOptions, _mockHttpService.Object);
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
            issuer: _mockJwtAuthConfig.Issuer,
            audience: _mockJwtAuthConfig.Audience,
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


        context.HttpContext.Response.Cookies.Append(OpenIdConnectCookies.OneLoginState, "state_value");
        context.HttpContext.Response.Cookies.Append(OpenIdConnectCookies.ParToken, "par_value");

        await _events.RedirectToIdentityProviderForSignOut(context);

        Assert.Null(context.HttpContext.Request.Cookies[OpenIdConnectCookies.OneLoginState]);
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
        Assert.Contains($"{OpenIdConnectCookies.OneLoginState}=test-state-value", setCookieHeaders);
    }
}

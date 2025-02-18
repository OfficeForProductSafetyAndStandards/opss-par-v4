# Authentication

This class library has been constructed to simplify the implementation of OIDC services such as GovUK One Login in future projects.

As it's been initially developed for PAR, the following details on registering the 
service are directly related to a Blazor web application and a .NET API backend.

## Front end
Registering the Authentication services with the front end requires:

### Modifications to Program.cs
#### Register config sections
```
var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
builder.Services.Configure<OpenIdConnectAuthConfig>(oneLoginAuthConfigSection);

var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
builder.Services.Configure<JwtAuthConfig>(jwtAuthConfigSection);
```

#### Add One Login Authentication:
```
builder.AddOneLoginAuthentication();
```

#### Register the IHttpService from Common Http
```
builder.Services.AddScoped<IHttpService, HttpService>();
```

### OidcController
The below controller will redirect the user to the appropriate OIDC provder for login
```
[ApiController]
[Route("oidc")]
public class OidcController : ControllerBase
{
    [HttpGet("login")]
    public ActionResult Login([FromQuery] Uri? returnUrl, [FromQuery] string provider = "oidc-onelogin")
    {
        if (returnUrl == null)
        {
            returnUrl = new Uri(HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase : "/", UriKind.Relative);
        }

        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl.ToString(),
        };

        return Challenge(props, provider);
    }

    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout([FromQuery] Uri? returnUrl, [FromQuery] string provider = "oidc-onelogin")
    {
        if (returnUrl == null)
        {
            returnUrl = new Uri(HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase : "/", UriKind.Relative);
        }

        var props = new AuthenticationProperties { RedirectUri = returnUrl.ToString() };
        return SignOut(props, CookieAuthenticationDefaults.AuthenticationScheme, provider);
    }
}
```

## API back end
Registering the Authentication services with the back end requires:

### Modifications to Program.cs
#### Register config sections
```
var oneLoginAuthConfigSection = builder.Configuration.GetSection("OneLoginAuth");
builder.Services.Configure<OpenIdConnectAuthConfig>(oneLoginAuthConfigSection);

var jwtAuthConfigSection = builder.Configuration.GetSection("JwtAuth");
builder.Services.Configure<JwtAuthConfig>(jwtAuthConfigSection);
```

#### Add One Login Authentication:
```
builder.AddOneLoginAuthentication();
```

#### Register the IHttpService from Common Http
```
builder.Services.AddScoped<IHttpService, HttpService>();
```

### Functionality to provide the Jwt token
```
public class GetJwtTokenQueryHandler : IRequestHandler<GetJwtTokenQuery, string>
{
    private readonly ITokenService _tokenService;
    private readonly IAuthenticatedUserService _oneLoginService;

    public GetJwtTokenQueryHandler(
        ITokenService tokenService,
        IAuthenticatedUserService oneLoginService)
    {
        _tokenService = tokenService;
        _oneLoginService = oneLoginService;
    }

    public async Task<string> Handle(GetJwtTokenQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _tokenService.ValidateTokenAsync(request.IdToken, cancellationToken).ConfigureAwait(false);
        var response = await _oneLoginService.GetUserInfo(request.AccessToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem.Detail);
        }

        var email = response.Result?.Email;

        var token = _tokenService.GenerateJwtToken(email);

        return token;
    }
}
```
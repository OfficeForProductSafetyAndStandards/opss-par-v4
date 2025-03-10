using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opss.PrimaryAuthorityRegister.Cqrs.Services;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Net;

namespace Opss.PrimaryAuthorityRegister.Web.Controllers;

[ApiController]
[Route("oidc")]
public class OidcController : ControllerBase
{
    private readonly ICqrsService _cqrsService;

    public OidcController(ICqrsService cqrsService)
    {
        _cqrsService = cqrsService;
    }

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

    [Authorize]
    [HttpGet("after-login")]
    public async Task<IActionResult> AfterLogin()
    {
        var profile = await _cqrsService.GetAsync<GetMyProfileQuery, MyProfileDto?>(new GetMyProfileQuery()).ConfigureAwait(true);

        if (profile.Result?.HasAcceptedTermsAndConditions ?? false)
        {
            return Redirect("/authority");
        }
        
        return Redirect("/terms-conditions");
    }
}
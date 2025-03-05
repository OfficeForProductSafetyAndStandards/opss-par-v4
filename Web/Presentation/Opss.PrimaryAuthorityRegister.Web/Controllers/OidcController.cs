using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Opss.PrimaryAuthorityRegister.Web.Controllers;

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

    [Authorize]
    [HttpGet("post-login")]
    public IActionResult PostLogin()
    {
        return Redirect("/authority");
    }
}
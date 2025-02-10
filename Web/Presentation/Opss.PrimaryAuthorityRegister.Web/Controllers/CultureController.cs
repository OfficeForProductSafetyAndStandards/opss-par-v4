using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Opss.PrimaryAuthorityRegister.Web.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, Uri redirectUri)
    {
        ArgumentNullException.ThrowIfNull(redirectUri);

        if (!ModelState.IsValid)
            return LocalRedirect("/");

        if (culture != null)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture, culture)));
        }

        return LocalRedirect(redirectUri.ToString());
    }
}
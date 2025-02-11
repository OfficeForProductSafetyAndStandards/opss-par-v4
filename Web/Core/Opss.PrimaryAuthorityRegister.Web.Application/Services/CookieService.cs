using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Services;

public class CookieService : ICookieService
{
    public void SetCultureCookie(HttpContext? httpContext)
    {
        if (httpContext == null) return;

        httpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture))
        );
    }
}

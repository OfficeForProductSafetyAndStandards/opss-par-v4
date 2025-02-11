using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCultureCookie()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        httpContext.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture))
        );
    }
}

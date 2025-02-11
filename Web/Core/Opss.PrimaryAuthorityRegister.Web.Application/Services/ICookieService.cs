using Microsoft.AspNetCore.Http;

namespace Opss.PrimaryAuthorityRegister.Web.Application.Services;

public interface ICookieService
{
    void SetCultureCookie(HttpContext? httpContext);
}

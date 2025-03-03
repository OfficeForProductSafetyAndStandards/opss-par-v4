using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Services;

/// <summary>
/// Service to prive access to the current user
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// The current claims principal from the HttpContext
    /// </summary>
    public ClaimsPrincipal? CurrentUser
    {
        get
        {
            if (_httpContextAccessor.HttpContext == null) return null;

            return _httpContextAccessor.HttpContext!.User;
        }
    }
}

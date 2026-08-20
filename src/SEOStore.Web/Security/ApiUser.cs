using System.Security.Claims;
using SEOStore.Domain.Identity;

namespace SEOStore.Web.Security;

internal static class ApiUser
{
    public static string? Id(ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);

    public static bool IsAdmin(ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Admin);
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SEOStore.Domain.Identity;

namespace SEOStore.Web.Security;

public sealed class AdminApiAttribute : AuthorizeAttribute
{
    public AdminApiAttribute()
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        Roles = AppRoles.Admin;
    }
}

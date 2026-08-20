using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace SEOStore.Infrastructure.Identity;

internal static class CookieAuthJson
{
    public static Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        => Handle(context, StatusCodes.Status401Unauthorized, "Authentication is required.");

    public static Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        => Handle(context, StatusCodes.Status403Forbidden, "An Admin JWT is required.");

    private static Task Handle(
        RedirectContext<CookieAuthenticationOptions> context,
        int statusCode,
        string message)
    {
        if (!WantsJson(context.Request))
        {
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { message });
    }

    private static bool WantsJson(HttpRequest request)
    {
        if (request.Path.StartsWithSegments("/api"))
            return true;

        var authorization = request.Headers.Authorization.ToString();
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return true;

        var accept = request.Headers.Accept.ToString();
        return accept.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            && !accept.Contains("text/html", StringComparison.OrdinalIgnoreCase);
    }
}

using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Web.Seo;

public static class SlugRedirects
{
    public static async Task<IActionResult?> PermanentIfFound(
        ControllerBase controller,
        ISlugRedirectService redirects,
        string requestedPath,
        CancellationToken cancellationToken)
    {
        var target = await redirects.ResolveLiveAsync(requestedPath, cancellationToken);
        return string.IsNullOrWhiteSpace(target) ? null : controller.RedirectPermanent(target);
    }
}

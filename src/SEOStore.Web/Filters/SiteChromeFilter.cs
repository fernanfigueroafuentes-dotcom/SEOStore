using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Web.Filters;

public sealed class SiteChromeFilter : IAsyncActionFilter
{
    private readonly ISettingService _settingService;
    private readonly ICategoryService _categoryService;
    private readonly IPageService _pageService;

    public SiteChromeFilter(ISettingService settingService, ICategoryService categoryService, IPageService pageService)
    {
        _settingService = settingService;
        _categoryService = categoryService;
        _pageService = pageService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is Controller controller)
        {
            var cancellation = context.HttpContext.RequestAborted;
            controller.ViewData["Site"] = await _settingService.GetCurrentAsync(cancellation);
            controller.ViewData["NavCategories"] = await _categoryService.GetPublishedAsync(cancellation);
            controller.ViewData["NavPages"] = await _pageService.GetPublishedAsync(cancellation);
        }

        await next();
    }
}

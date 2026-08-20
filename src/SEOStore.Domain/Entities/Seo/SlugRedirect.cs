using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Seo;

public class SlugRedirect : BaseEntity
{
    public string OldPath { get; set; } = string.Empty;

    public string NewPath { get; set; } = string.Empty;
}

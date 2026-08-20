using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Admin;

public class CategoryForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [MaxLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Título SEO")]
    [MaxLength(70)]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta descripción")]
    [MaxLength(160)]
    public string? MetaDescription { get; set; }

    [Display(Name = "Publicada")]
    public bool Published { get; set; } = true;

    [Display(Name = "Permitir indexación")]
    public bool Index { get; set; } = true;

    [Display(Name = "Seguir enlaces")]
    public bool Follow { get; set; } = true;

    [Display(Name = "Orden")]
    public int DisplayOrder { get; set; }
}

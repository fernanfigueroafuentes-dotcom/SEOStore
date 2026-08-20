using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Admin;

public class PageForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [Display(Name = "Título")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Slug")]
    [MaxLength(200)]
    public string? Slug { get; set; }

    [Display(Name = "Contenido")]
    public string? Content { get; set; }

    [Display(Name = "Publicada")]
    public bool Published { get; set; } = true;

    [Display(Name = "Título SEO")]
    [MaxLength(70)]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta descripción")]
    [MaxLength(160)]
    public string? MetaDescription { get; set; }

    [Display(Name = "Permitir indexación")]
    public bool Index { get; set; } = true;

    [Display(Name = "Seguir enlaces")]
    public bool Follow { get; set; } = true;
}

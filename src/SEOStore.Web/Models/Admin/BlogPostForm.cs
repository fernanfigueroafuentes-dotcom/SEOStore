using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Admin;

public class BlogPostForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [Display(Name = "Título")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Slug")]
    [MaxLength(200)]
    public string? Slug { get; set; }

    [Display(Name = "Resumen")]
    [MaxLength(500)]
    public string? Summary { get; set; }

    [Display(Name = "Contenido")]
    public string? Content { get; set; }

    [Display(Name = "Imagen destacada (URL)")]
    [MaxLength(500)]
    public string? FeaturedImageUrl { get; set; }

    [Display(Name = "Autor")]
    [MaxLength(150)]
    public string? Author { get; set; }

    [Display(Name = "Orden")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Publicada")]
    public bool Published { get; set; }

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

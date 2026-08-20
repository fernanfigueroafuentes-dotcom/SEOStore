using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Admin;

public class BannerForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [Display(Name = "Título")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Subtítulo")]
    [MaxLength(300)]
    public string? Subtitle { get; set; }

    [Display(Name = "Imagen (URL)")]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Display(Name = "Foto")]
    public IFormFile? Photo { get; set; }

    [Display(Name = "Enlace")]
    [MaxLength(500)]
    public string? Link { get; set; }

    [Display(Name = "Orden")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Activo")]
    public bool Active { get; set; } = true;
}

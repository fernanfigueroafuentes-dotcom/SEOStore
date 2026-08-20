using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Admin;

public class BrandForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Logo (URL)")]
    [MaxLength(500)]
    public string? LogoUrl { get; set; }
}

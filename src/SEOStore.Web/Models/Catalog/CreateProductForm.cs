using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SEOStore.Web.Models.Catalog;

public class CreateProductForm
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    [MaxLength(50)]
    public string? SKU { get; set; }

    [Display(Name = "Descripción corta")]
    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    [Display(Name = "Descripción del producto")]
    public string? Description { get; set; }

    [Display(Name = "Precio")]
    [Range(0, 9999999)]
    public decimal Price { get; set; }

    [Display(Name = "Mostrar precio")]
    public bool ShowPrice { get; set; }

    [Display(Name = "Publicado")]
    public bool Published { get; set; } = true;

    [Display(Name = "Mensaje de WhatsApp")]
    [MaxLength(500)]
    public string? WhatsAppMessage { get; set; }

    [Display(Name = "Categoría")]
    public int? CategoryId { get; set; }

    [Display(Name = "Nueva categoría")]
    [MaxLength(100)]
    public string? NewCategoryName { get; set; }

    [Display(Name = "Título SEO de la categoría")]
    [MaxLength(70)]
    public string? NewCategoryMetaTitle { get; set; }

    [Display(Name = "Meta descripción de la categoría")]
    [MaxLength(160)]
    public string? NewCategoryMetaDescription { get; set; }

    [Display(Name = "Marca")]
    public int? BrandId { get; set; }

    [Display(Name = "Nueva marca")]
    [MaxLength(100)]
    public string? NewBrandName { get; set; }

    [Display(Name = "Fotos")]
    public List<IFormFile> Photos { get; set; } = [];

    [Display(Name = "Texto alternativo de las fotos")]
    [MaxLength(200)]
    public string? PhotoAlt { get; set; }

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

    [Display(Name = "Destacado en inicio")]
    public bool Featured { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = [];

    public IEnumerable<SelectListItem> Brands { get; set; } = [];
}

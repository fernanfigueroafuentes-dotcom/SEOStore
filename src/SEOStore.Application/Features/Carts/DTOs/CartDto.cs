namespace SEOStore.Application.Features.Carts.DTOs;

public class CartDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public List<CartItemDto> Items { get; set; } = [];

    public decimal Total => Items.Sum(x => x.Total);
}

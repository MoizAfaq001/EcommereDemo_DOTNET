using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class CartItemDTO
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal Total => Price * Quantity;
    }
}

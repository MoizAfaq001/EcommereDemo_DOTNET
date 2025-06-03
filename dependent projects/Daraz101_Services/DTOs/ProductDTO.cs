using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsFeatured { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}

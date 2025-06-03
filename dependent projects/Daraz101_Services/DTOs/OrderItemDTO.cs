using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }  // Added

        [Required]
        public string ProductName { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public decimal Total => Quantity * UnitPrice;
    }
}

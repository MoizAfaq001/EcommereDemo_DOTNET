using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        [Required]
        public string DeliveryAddress { get; set; } = null!;

        public string Status { get; set; } = "Pending";
        public string UserId { get; set; } = null!;

        [Required]
        public List<OrderItemDTO> Items { get; set; } = new();

        public decimal OrderTotal => Items.Sum(i => i.Total);
    }
}

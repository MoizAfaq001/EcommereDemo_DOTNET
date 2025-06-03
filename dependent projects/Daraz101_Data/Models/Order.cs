using System.ComponentModel.DataAnnotations;

namespace Daraz101_Data
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";

        [Required]
        public int AddressId { get; set; }

        [Required]
        public required Address Address { get; set; }  

        public required ICollection<OrderItem> Items { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;


namespace Daraz101_Data
{
    public class CartItem
    {
        public int Id { get; set; }

        public required string UserId { get; set; }

        public required ApplicationUser User { get; set; }

        [Required]
        public int ProductId { get; set; }
        public required Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}

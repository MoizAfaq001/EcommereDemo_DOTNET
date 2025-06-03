using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class UserProfileDTO
    {
        public string UserId { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? FullAddress { get; set; }

        public List<OrderDTO> OrderHistory { get; set; } = new();

        public List<AddressDTO>? Addresses { get; set; } // Added for multiple addresses
    }
}

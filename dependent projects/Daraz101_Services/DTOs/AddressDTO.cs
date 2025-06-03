using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services
{
    public class AddressDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Street { get; set; } = null!;

        [Required, MaxLength(50)]
        public string City { get; set; } = null!;

        [Required, MaxLength(20)]
        public string PostalCode { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Country { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}

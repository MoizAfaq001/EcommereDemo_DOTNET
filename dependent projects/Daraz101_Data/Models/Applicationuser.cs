using Microsoft.AspNetCore.Identity;

namespace Daraz101_Data
{

        public class ApplicationUser : IdentityUser
        {
            public required string? FullName { get; set; }

            public ICollection<Address> Addresses { get; set; } = new List<Address>();
            public ICollection<Order> Orders { get; set; } = new List<Order>();
            public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }

}

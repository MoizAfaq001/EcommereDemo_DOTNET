using System.ComponentModel.DataAnnotations;

namespace Daraz101_Services.DTOs
{
    public class RegisterDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        public string? Password { get; set; } 

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }  
    }

}

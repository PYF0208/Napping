using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models
{
    public class CustomerViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;        
    }
}

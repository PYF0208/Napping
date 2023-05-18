using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models
{
    public class EmailValidViewModel
    {
        public EmailValidViewModel(string _Email)
        {
            Email = _Email;
        }
        [Required(ErrorMessage = "Email為必填欄位")]
        [EmailAddress(ErrorMessage = "需符合Email格式")]
        public string Email { get; set; } = null!;
    }
}

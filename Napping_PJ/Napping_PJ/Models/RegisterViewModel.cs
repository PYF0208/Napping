using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email為必填欄位")]
        [EmailAddress(ErrorMessage ="需符合Email格式")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage ="密碼為必填欄位")]
        [Display(Name ="密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "密碼確認為必填欄位")]
        [Display(Name = "密碼確認")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不匹配。")]
        public string ConfirmPassword { get; set; } = null!;       
    }
}

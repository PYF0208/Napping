using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Areas.Admin.Models
{
    public class RoleViewModel
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [Display(Name="角色名稱")]
        public string Name { get; set; } = null!;
    }
}

using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models
{
    public class UserRoleViewModel
    {
        //public int RoleId { get; set; }
        //public string RoleName { get; set; }
        public Customer customer { get; set; }
        //public int CustomerId { get; set; }
        //public string CustomerName { get; set; }
        //public IList<Role> AllRoleList { get; set; }
        public IList<Role> SelectedRole { get; set; }
    }
}

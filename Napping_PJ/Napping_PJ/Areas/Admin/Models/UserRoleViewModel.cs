using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models
{
    public class UserRoleViewModel
    {
        public Customer customer { get; set; }
        public IList<Role> SelectedRole { get; set; }
    }
}

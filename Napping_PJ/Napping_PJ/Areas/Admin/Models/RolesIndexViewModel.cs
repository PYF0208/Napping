using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models
{
    public class RolesIndexViewModel
    {
        public IEnumerable<UserRoleViewModel> UserRoleViewModels { get; set; }
        public IEnumerable<Role> Roles { get; set; }
    }
}

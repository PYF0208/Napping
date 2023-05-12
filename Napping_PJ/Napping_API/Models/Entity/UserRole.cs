using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class UserRole
    {
        public int RoleCustomerId { get; set; }
        public int RoleId { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}

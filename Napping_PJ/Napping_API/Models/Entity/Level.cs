using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Level
    {
        public Level()
        {
            Customers = new HashSet<Customer>();
            Promotions = new HashSet<Promotion>();
        }

        public int LevelId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}

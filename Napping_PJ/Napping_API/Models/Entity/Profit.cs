using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Profit
    {
        public Profit()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProfitId { get; set; }
        public DateTime Date { get; set; }
        public double Number { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

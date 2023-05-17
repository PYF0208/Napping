using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Currency
    {
        public Currency()
        {
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
        }

        public int CurrencyId { get; set; }
        public string Name { get; set; } = null!;
        public string? Symbol { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}

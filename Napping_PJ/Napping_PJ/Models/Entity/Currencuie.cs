using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Currencuie
    {
        public Currencuie()
        {
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
        }

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = null!;
        public string? CurrencySymbol { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = null!;
        public int CurrencyId { get; set; }
        public string Status { get; set; } = null!;

        public virtual Currency Currency { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}

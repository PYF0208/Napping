using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime Date { get; set; }
        public int PaymentId { get; set; }

        public virtual Currency Currency { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual Payment Payment { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}

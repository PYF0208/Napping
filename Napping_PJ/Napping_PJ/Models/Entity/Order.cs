using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public int PaymentId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Payment Payment { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = null!;
        public int Status { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}

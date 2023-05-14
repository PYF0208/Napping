using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class CustomerGift
    {
        public int CustomerId { get; set; }
        public int GiftId { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Gift Gift { get; set; } = null!;
    }
}

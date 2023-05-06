using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class BellEvent
    {
        public int BellEventId { get; set; }
        public int CustomerId { get; set; }
        public string EventContent { get; set; } = null!;
        public bool IsRead { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Like
    {
        public int RoomId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? EndTime { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}

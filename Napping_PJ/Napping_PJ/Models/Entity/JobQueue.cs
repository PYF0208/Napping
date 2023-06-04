using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class JobQueue
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string Queue { get; set; } = null!;
        public DateTime? FetchedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class List
    {
        public long Id { get; set; }
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}

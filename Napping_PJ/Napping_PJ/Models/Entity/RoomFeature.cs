using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class RoomFeature
    {
        public int RoomId { get; set; }
        public int FeatureId { get; set; }
        public int RoomfeatureId { get; set; }

        public virtual Feature Feature { get; set; } = null!;
        public virtual Room Room { get; set; } = null!;
    }
}

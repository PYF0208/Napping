using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Feature
    {
        public Feature()
        {
            RoomFeatures = new HashSet<RoomFeature>();
        }

        public int FeatureId { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;

        public virtual ICollection<RoomFeature> RoomFeatures { get; set; }
    }
}

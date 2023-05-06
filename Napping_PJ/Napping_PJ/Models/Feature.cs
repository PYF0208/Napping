using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class Feature
    {
        public Feature()
        {
            Rooms = new HashSet<Room>();
        }

        public int FeatureId { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;

        public virtual ICollection<Room> Rooms { get; set; }
    }
}

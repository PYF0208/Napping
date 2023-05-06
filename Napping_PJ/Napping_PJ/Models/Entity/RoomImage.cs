using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class RoomImage
    {
        public int RoomImageId { get; set; }
        public int RoomId { get; set; }
        public string Image { get; set; } = null!;

        public virtual Room Room { get; set; } = null!;
    }
}

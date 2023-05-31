using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Room
    {
        public Room()
        {
            RoomFeatures = new HashSet<RoomFeature>();
            RoomImages = new HashSet<RoomImage>();
        }

        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int? Number { get; set; }
        public string Type { get; set; } = null!;
        public double Price { get; set; }
        public int MaxGuests { get; set; }

        public virtual Hotel Hotel { get; set; } = null!;
        public virtual ICollection<RoomFeature> RoomFeatures { get; set; }
        public virtual ICollection<RoomImage> RoomImages { get; set; }
    }
}

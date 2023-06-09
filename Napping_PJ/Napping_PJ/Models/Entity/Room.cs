﻿using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Room
    {
        public Room()
        {
            OrderDetails = new HashSet<OrderDetail>();
            RoomImages = new HashSet<RoomImage>();
            Features = new HashSet<Feature>();
        }

        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public int? Number { get; set; }
        public string Type { get; set; } = null!;
        public double Price { get; set; }
        public int MaxGuests { get; set; }

        public virtual Hotel Hotel { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<RoomImage> RoomImages { get; set; }

        public virtual ICollection<Feature> Features { get; set; }
    }
}

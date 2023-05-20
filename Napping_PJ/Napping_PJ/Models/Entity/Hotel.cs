using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Hotel
    {
        public Hotel()
        {
            Comments = new HashSet<Comment>();
            ExtraServices = new HashSet<ExtraService>();
            Rooms = new HashSet<Room>();
        }

        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string Star { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public double? PositionLat { get; set; }
        public double? PositionLon { get; set; }
        public string Address { get; set; } = null!;
        public string? Description { get; set; }
        public double? AvgComment { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ExtraService> ExtraServices { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}

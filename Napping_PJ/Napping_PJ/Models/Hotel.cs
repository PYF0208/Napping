using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
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
        public decimal Phone { get; set; }
        public string Email { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double? AvgComment { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<ExtraService> ExtraServices { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}

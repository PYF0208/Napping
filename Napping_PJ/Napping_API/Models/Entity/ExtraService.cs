using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class ExtraService
    {
        public ExtraService()
        {
            OrderDetailExtraServices = new HashSet<OrderDetailExtraService>();
        }

        public int ExtraServiceId { get; set; }
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }

        public virtual Hotel Hotel { get; set; } = null!;
        public virtual ICollection<OrderDetailExtraService> OrderDetailExtraServices { get; set; }
    }
}

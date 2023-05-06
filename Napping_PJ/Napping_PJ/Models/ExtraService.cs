using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class ExtraService
    {
        public int ExtraServiceId { get; set; }
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }

        public virtual Hotel Hotel { get; set; } = null!;
    }
}

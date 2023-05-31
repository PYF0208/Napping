using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class ExtraService
    {
        public ExtraService()
        {
            HotelExtraServices = new HashSet<HotelExtraService>();
        }

        public int ExtraServiceId { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;

        public virtual ICollection<HotelExtraService> HotelExtraServices { get; set; }
    }
}

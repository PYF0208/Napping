﻿using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class HotelExtraService
    {
        public int Hesid { get; set; }
        public int HotelId { get; set; }
        public int ExtraServiceId { get; set; }
        public int ServicePrice { get; set; }

        public virtual ExtraService ExtraService { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}

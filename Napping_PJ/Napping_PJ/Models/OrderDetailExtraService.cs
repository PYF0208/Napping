using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class OrderDetailExtraService
    {
        public string OidRidPid { get; set; } = null!;
        public int ExtraServiceId { get; set; }
        public int Number { get; set; }

        public virtual ExtraService ExtraService { get; set; } = null!;
        public virtual OrderDetail OidRidP { get; set; } = null!;
    }
}

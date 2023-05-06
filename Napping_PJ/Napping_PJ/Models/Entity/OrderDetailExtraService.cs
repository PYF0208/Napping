using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class OrderDetailExtraService
    {
        public int OidRidPid { get; set; }
        public int ExtraServiceId { get; set; }
        public int Number { get; set; }

        public virtual ExtraService ExtraService { get; set; } = null!;
        public virtual OrderDetail OidRidP { get; set; } = null!;
    }
}

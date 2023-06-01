using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class OrderDetailExtraService
    {
        public int Odesid { get; set; }
        public int OrderDetailId { get; set; }
        public int ExtraServiceName { get; set; }
        public int Number { get; set; }

        public virtual OrderDetail OrderDetail { get; set; } = null!;
    }
}

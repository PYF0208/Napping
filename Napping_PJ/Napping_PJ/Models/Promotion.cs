using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class Promotion
    {
        public int PromotionId { get; set; }
        public int LevelId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Discount { get; set; }

        public virtual Level Level { get; set; } = null!;
    }
}

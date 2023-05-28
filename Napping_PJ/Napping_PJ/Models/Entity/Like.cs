using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Like
    {
        public int HotelId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}

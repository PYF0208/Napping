using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Gift
    {
        public Gift()
        {
            CustomerGifts = new HashSet<CustomerGift>();
        }

        public int GiftId { get; set; }
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public int ValidDate { get; set; }

        public virtual ICollection<CustomerGift> CustomerGifts { get; set; }
    }
}

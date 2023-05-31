using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            OrderDetailExtraServices = new HashSet<OrderDetailExtraService>();
        }

        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int RoomId { get; set; }
        public int ProfitId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public string TravelType { get; set; } = null!;
        public string? Note { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ICollection<OrderDetailExtraService> OrderDetailExtraServices { get; set; }
    }
}

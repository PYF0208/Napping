using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string NameOfBooking { get; set; } = null!;
        public string PhoneOfBooking { get; set; } = null!;
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public string PaymentType { get; set; } = null!;

        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}

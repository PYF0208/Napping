using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models
{
	public class OrdersViewModel
	{
		public int OrderId { get; set; }
		public int CustomerId { get; set; }
		public DateTime Date { get; set; }
		public int PaymentId { get; set; }
        public string PaymentType { get; set; } = null!;
        public string? CustomerName { get; set; }
        public virtual Customer Customer { get; set; } = null!;
		public virtual Payment Payment { get; set; } = null!;

        

        /* 訂單明細 */

        public int OrderDetailId { get; set; }
        

        public DateTime CheckIn { get; set; } //入住時間
        public DateTime CheckOut { get; set; } //退房時間
        public int NumberOfGuests { get; set; } //客人人數
        public string TravelType { get; set; } = null!;  //旅行類型
        public string? Note { get; set; } //筆記

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

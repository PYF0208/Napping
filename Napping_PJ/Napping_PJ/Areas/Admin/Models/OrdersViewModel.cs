using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models
{
	public class OrdersViewModel
	{
		public int OrderId { get; set; }
		public int CustomerId { get; set; }
		public DateTime Date { get; set; }
		public int PaymentId { get; set; }

		public string? CustomerName { get; set; }
        public virtual Customer Customer { get; set; } = null!;
		public virtual Payment Payment { get; set; } = null!;
	}
}

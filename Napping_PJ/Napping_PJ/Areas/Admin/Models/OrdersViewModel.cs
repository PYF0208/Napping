namespace Napping_PJ.Areas.Admin.Models
{
	public class OrdersViewModel
	{
		public int OrderId { get; set; }
		public int CustomerId { get; set; }
		public int CurrencyId { get; set; }
		public DateTime Date { get; set; }
		public int PaymentId { get; set; }
	}
}

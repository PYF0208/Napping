namespace Napping_PJ.Areas.Admin.Models
{
	public class PaymentViewModel
	{
		public int PaymentId { get; set; }
		public int OrderId { get;	set; }
		public DateTime Date { get; set; }
		public string Type { get; set; } = null!;
		
		public int Status { get; set; } 
	}
}

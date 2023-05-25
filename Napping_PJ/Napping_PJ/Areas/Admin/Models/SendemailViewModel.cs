namespace Napping_PJ.Areas.Admin.Models
{
	public class SendemailViewModel
	{
		public int PromotionId { get; set; }

		public string PromotionName { get; set; } = null!;

		public string LevelName { get; set; } = null!;

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public double Discount { get; set; }
	}
}

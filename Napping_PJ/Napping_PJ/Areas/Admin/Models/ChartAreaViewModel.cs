namespace Napping_PJ.Areas.Admin.Models
{
	public class ChartAreaViewModel
	{
		public int OrderId { get; set; }
		public DateTime Date { get; set; }
		public double TotalPrice { get; set; }
	}

	public class ChartBarViewModel {

		public string City { get; set; }
		public int Count { get; set; }

	}


}

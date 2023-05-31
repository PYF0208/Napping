namespace Napping_PJ.Areas.Admin.Models
{
	public class HotelsViewModel
	{
		public int HotelId { get; set; }
		public string Name { get; set; } = null!;
		public string Star { get; set; } = null!;
		public string Image { get; set; } = null!;
		public string ContactName { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Region { get; set; } = null!;
		public string Address { get; set; } = null!;
		public double? AvgComment { get; set; }


	}
}

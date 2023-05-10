namespace Napping_PJ.Areas.Admin.Models
{
	public class HotelsViewModel
	{
		public int HotelId { get; set; }
		public string? Name { get; set; }
		public string? Star { get; set; } 
		public string? Image { get; set; }
		public string? ContactName { get; set; } 
		public decimal Phone { get; set; }
		public string? Email { get; set; }
		public string? City { get; set; } 
		public string? Region { get; set; }
		public string? Address { get; set; } 
		public double? AvgComment { get; set; }
	}
}

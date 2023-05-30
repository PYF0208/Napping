namespace Napping_PJ.Models
{
	public class HotelPageViewModel
	{
		public int HotelId { get; set; }
		public string Name { get; set; } = null!;
		public string Star { get; set; } = null!;
		public string Image { get; set; } = null!;
		public string ContactName { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Region { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string? Description { get; set; }
	}
}

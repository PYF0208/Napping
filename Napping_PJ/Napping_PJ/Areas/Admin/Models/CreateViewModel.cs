namespace Napping_PJ.Areas.Admin.Models
{
	public class CreateViewModel
	{
		public int HotelId { get; set; }
		public string? Type { get; set; }
		public double Price { get; set; }
		public int MaxGuests { get; set; }

		public List<string> Pic { get; set; }
	}
}

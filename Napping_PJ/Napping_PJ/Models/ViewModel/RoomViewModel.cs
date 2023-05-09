namespace Napping_PJ.Models.ViewModel
{
	public class RoomViewModel
	{
		public int RoomId { get; set; }
		public int HotelId { get; set; }
		public string? Type { get; set; } 
		public double Price { get; set; }
		public int MaxGuests { get; set; }
	}
}

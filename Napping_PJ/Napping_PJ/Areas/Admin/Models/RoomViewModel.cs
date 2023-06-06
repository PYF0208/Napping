using Napping_PJ.Models;

namespace Napping_PJ.Areas.Admin.Models
{
	public class RoomsViewModel
	{
		public int RoomId { get; set; }
		public int HotelId { get; set; }
		public string? Type { get; set; }
		public double Price { get; set; }
		public int MaxGuests { get; set; }
		public string Name { get; set; }

		public List<ImageInRoomViewModel> pic { get; set; }
		public List<FeatureInRoomViewModel> Feature { get; set; }
		
	}
	
	public class FeatureInRoomViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
	}
	public class ImageInRoomViewModel
	{
		public string Image { get; set; }
	}
}

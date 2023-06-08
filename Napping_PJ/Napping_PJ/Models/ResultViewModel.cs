using MessagePack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models
{
	public class ResultViewModel
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int HotelId { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public string City { get; set; }
		public string Region { get; set; }
		public double Price { get; set; }
		public int MaxGuests { get; set; }
		public float PositionLat { get; set; }
		public float PositionLon { get; set; }
		public bool IsLike { get; set; }
		public int RoomId { get; set; }
		public List<RoomOrderViewModel>? RoomOrders { get; set; }

	}
	public class RoomOrderViewModel
	{
		public int RoomId { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
	}
}


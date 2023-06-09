﻿namespace Napping_PJ.Areas.Admin.Models
{
	public class RoomsEditViewModel
	{
		public int RoomId { get; set; }
		public int HotelId { get; set; }
		public string? Type { get; set; }
		public double Price { get; set; }
		public int MaxGuests { get; set; }

		public List<int> Features{ get; set; }
	}
}

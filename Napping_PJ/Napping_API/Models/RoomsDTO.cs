﻿namespace Napping_API.Models
{
   public class RoomsDTO
	{
		public int RoomId { get; set; }
		public int HotelId { get; set; }
		public string? Type { get; set; } 
		public double Price { get; set; }
		public int MaxGuests { get; set; }
	}
}

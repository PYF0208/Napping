namespace Napping_PJ.Models
{
	public class LikeViewModel
	{
		public int HotelId { get; set; }
		public string HotelName { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Region { get; set; } = null!;
		public string HotelImage { get; set; } = null!;
        public bool IsLike { get; set; }

        public DateTime CreateDate { get; set; }
	}
}

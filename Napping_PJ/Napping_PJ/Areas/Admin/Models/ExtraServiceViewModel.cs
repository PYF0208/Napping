namespace Napping_PJ.Areas.Admin.Models
{
    public class ExtraServiceViewModel
    {

        public int ExtraServiceId { get; set; }
    
        public string ExtraServiceName { get; set; } = null!;
		public string? Image { get; set; }

		public int HotelId { get; set; }
		public string HotelName { get; set; } = null!;

		public double ExtraServicePrice { get; set; }






	}
}

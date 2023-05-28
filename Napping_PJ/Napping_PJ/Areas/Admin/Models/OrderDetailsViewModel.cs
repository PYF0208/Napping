namespace Napping_PJ.Areas.Admin.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderDetailId { get; set; }
        
        public int RoomId { get; set; }
        public int ProfitId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public string TravelType { get; set; } = null!;
        public string? Note { get; set; }
        /*訂單*/
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
    }
}

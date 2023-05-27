using Napping_PJ.Models.Entity;

namespace Napping_PJ.Models
{
    public class CartViewModel
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int MaxGuests { get; set; }
        public int TravelType { get; set; }
        public string Note { get; set; }
        public List<SelectedExtraServiceViewModel> SelectedExtraServices { get; set; }
    }
    public class SelectedExtraServiceViewModel
    {
        public int ExtraServiceId { get; set; }
        public string Name { get; set; }
        public int ServiceQuantity { get; set; }
    }
}

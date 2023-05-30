using Napping_PJ.Models.Entity;

namespace Napping_PJ.Models
{
    public class RoomDetailViewModel
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public string HotelName { get; set; }
        public ICollection<RoomImage>? RoomImages { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int MaxGuests { get; set; }
        public int TravelType { get; set; }
        public string Note { get; set; }
        public IEnumerable<RoomFeatureViewModel> RoomFeatures { get; set; }
        public List<SelectedExtraServiceViewModel> SelectedExtraServices { get; set; }
    }
    public class SelectedExtraServiceViewModel
    {
        public int ExtraServiceId { get; set; }
        public string Name { get; set; }
        public int ServiceQuantity { get; set; }
    }
    public class RoomFeatureViewModel
    {
        public int FeatureId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
}

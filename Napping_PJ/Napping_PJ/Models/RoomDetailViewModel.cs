using Napping_PJ.Models.Entity;

namespace Napping_PJ.Models
{
    public class RoomDetailViewModel
    {
        public int roomId { get; set; }
        public string roomType { get; set; }
        public string hotelName { get; set; }
        public ICollection<roomImagesViewModel>? roomImages { get; set; }
        public int availableCheckInTime { get; set; }
        public int latestCheckOutTime { get; set; }
        public DateTime? checkIn { get; set; }
        public DateTime? checkOut { get; set; }
        public int maxGuests { get; set; }
        public int travelType { get; set; }
        public double roomPrice { get; set; }
        public string note { get; set; }
        public int totalPrice { get; set; }
        public IEnumerable<roomFeatureViewModel> roomFeatures { get; set; }
        public List<selectedExtraServiceViewModel> selectedExtraServices { get; set; }
        public Dictionary<long, double> profitDictionary { get; set; }
    }
    public class selectedExtraServiceViewModel
    {
        public int extraServiceId { get; set; }
        public string name { get; set; }
        public string serviceImage { get; set; }
        public int servicePrice { get; set; }
        public int serviceQuantity { get; set; }
    }
    public class roomFeatureViewModel
    {
        public int featureId { get; set; }
        public string name { get; set; }
        public string image { get; set; }
    }

    public class roomImagesViewModel
    {
        public string image { get; set; }
    }
}

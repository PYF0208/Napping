namespace Napping_PJ.Models
{
    public class promotionViewModel
    {
        public int levelId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string name { get; set; } = null!;
        public double discount { get; set; }
    }
}

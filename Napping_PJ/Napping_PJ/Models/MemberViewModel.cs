namespace Napping_PJ.Models
{
    public class MemberViewModel
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Phone { get; set; }
        public bool? Gender { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public int? LevelId { get; set; }
        public string LevelName { get; set; }
        public int CustomerId { get; set; }
    }
}

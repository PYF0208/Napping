﻿namespace Napping_PJ.Models
{
    public class CommentViewModel
    {
        public int CommentId { get; set; }
        public int HotelId { get; set; }
        public int CustomerId { get; set; }
        public int Cp { get; set; }
        public int Comfortable { get; set; }
        public int Staff { get; set; }
        public int Facility { get; set; }
        public int Clean { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public int OrderId { get; set; }
    }
}

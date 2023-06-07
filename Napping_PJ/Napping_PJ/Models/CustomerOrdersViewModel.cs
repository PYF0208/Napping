namespace Napping_PJ.Models
{
    public class CustomerOrdersViewModel
    {
        public string NameOfBooking { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case 1:
                        return "未付款";
                    case 2:
                        return "已付款";
                    case 3:
                        return "已取消";
                    default:
                        return "未知狀態";
                }
            }
        }
        //Orders表

        public int OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NumberOfGuests { get; set; }
        public double? TotalPrice { get; set; }
        public string? Note { get; set; }
        //OrderDetails表

        public string RoomType { get; set; } = null!;
        public int HotelId { get; set; }
        //Rooms表

        public string HotelName { get; set; } = null!;
        public string HotelImage { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public double? AvgComment { get; set; }
        public string HotelPhone { get; set; } = null!;
        //Hotels表

        public string PaymentType { get; set; } = null!;
        //Payments表
    }
}

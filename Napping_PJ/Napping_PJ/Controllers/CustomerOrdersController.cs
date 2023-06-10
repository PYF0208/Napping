using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Security.Claims;
using System.Xml;

namespace Napping_PJ.Controllers
{
    [Authorize]
    public class CustomerOrdersController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public CustomerOrdersController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Star()
        {
            return View();
        }

        [HttpGet]
        public IEnumerable<CustomerOrdersViewModel> GetCustomerOrders()
        {
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var customerOrders = _context.OrderDetails.Include(x => x.Order).Include(x => x.Room).ThenInclude(x => x.Hotel)
                .Where(x => x.Order.CustomerId == customer.CustomerId).OrderByDescending(x => x.OrderId).Select(co => new CustomerOrdersViewModel
                {   //OrderDetails表
                    OrderId = co.OrderId,
                    OrderDetailId = co.OrderDetailId,
                    RoomId = co.RoomId,
                    CheckIn = co.CheckIn,
                    CheckOut = co.CheckOut,
                    NumberOfGuests = co.NumberOfGuests,
                    TotalPrice = co.RoomTotalPrice + co.EspriceTotal - co.DiscountTotalPrice,
                    Note = co.Note,

                    //Orders表
                    NameOfBooking = co.Order.NameOfBooking,
                    OrderDate = co.Order.Date,
                    Status = co.Order.Status,

                    //Rooms表
                    RoomType = co.Room.Type,

                    //Hotels表
                    HotelName = co.Room.Hotel.Name,
                    HotelImage = co.Room.Hotel.Image,
                    City = co.Room.Hotel.City,
                    Region = co.Room.Hotel.Region,
                    AvgComment = co.Room.Hotel.AvgComment,
                    HotelPhone = co.Room.Hotel.Phone,

                    //Payments表
                    PaymentType = co.Order.Payments.OrderBy(x => x.PaymentId).Last().Type
                });

            return customerOrders;
        }

        [HttpGet]
        public IActionResult FilterCustomerOrders(int orderId)
        {
            if (orderId == 0)
            {
                return Content("請輸入正確訂單編號!");
            }
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var customerOrders = _context.OrderDetails.Include(x => x.Order).Include(x => x.Room).ThenInclude(x => x.Hotel).Where(
                x => x.Order.CustomerId == customer.CustomerId && x.OrderId == orderId).Select(co => new CustomerOrdersViewModel
                {   //OrderDetails表
                    OrderId = co.OrderId,
                    OrderDetailId = co.OrderDetailId,
                    RoomId = co.RoomId,
                    CheckIn = co.CheckIn,
                    CheckOut = co.CheckOut,
                    NumberOfGuests = co.NumberOfGuests,
                    TotalPrice = co.RoomTotalPrice + co.EspriceTotal - co.DiscountTotalPrice,
                    Note = co.Note,

                    //Orders表
                    NameOfBooking = co.Order.NameOfBooking,
                    OrderDate = co.Order.Date,
                    Status = co.Order.Status,

                    //Rooms表
                    RoomType = co.Room.Type,

                    //Hotels表
                    HotelName = co.Room.Hotel.Name,
                    HotelImage = co.Room.Hotel.Image,
                    City = co.Room.Hotel.City,
                    Region = co.Room.Hotel.Region,
                    AvgComment = co.Room.Hotel.AvgComment,
                    HotelPhone = co.Room.Hotel.Phone,

                    //Payments表
                    PaymentType = co.Order.Payments.OrderBy(x => x.PaymentId).Last().Type
                });
            if (!customerOrders.Any())
            {
                return Content("查無此訂單編號!");
            }
            return Ok(customerOrders);

        }

        [HttpPost]
        public string ShowCheckOut(int status, int orderId)
        {
            if (status == 1)
            {
                //return $"<a href=\"/CheckOut/IndexByOrder?orderId={orderId}\" class=\"btn btn-primary\">跳轉結帳頁面</a>";
                return $"<form class=\"needs-validation\" novalidate=\"\" method=\"post\" action=\"/Bank/SpgatewayPayBill\">" +
                    $"<input style=\"display:none;\" type=\"text\" class=\"form-control\" id=\"orderId\" name=\"orderId\" placeholder=\"王小明\" value=\"{orderId}\" required=\"\">" +
                    "<button class=\"btn btn-primary btn-lg btn-block\" type=\"submit\">跳轉結帳頁面</button>" +
                    "</form>";
            }
            return string.Empty;
        }

        [HttpGet]
        public IEnumerable<CustomerOrdersViewModel> GetFinishOrders()
        {
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var customerOrders = _context.OrderDetails.Include(x => x.Order).Include(x => x.Room).ThenInclude(x => x.Hotel)
                .Where(x => x.Order.CustomerId == customer.CustomerId && x.Order.Status == 2 && x.CheckOut < DateTime.Now).OrderByDescending(x => x.OrderId).Select(co => new CustomerOrdersViewModel
                {   //OrderDetails表
                    OrderId = co.OrderId,
                    OrderDetailId = co.OrderDetailId,
                    RoomId = co.RoomId,
                    CheckIn = co.CheckIn,
                    CheckOut = co.CheckOut,
                    NumberOfGuests = co.NumberOfGuests,
                    TotalPrice = co.RoomTotalPrice + co.EspriceTotal - co.DiscountTotalPrice,
                    Note = co.Note,

                    //Orders表
                    NameOfBooking = co.Order.NameOfBooking,
                    OrderDate = co.Order.Date,
                    Status = co.Order.Status,

                    //Rooms表
                    RoomType = co.Room.Type,
                    HotelId = co.Room.HotelId,

                    //Hotels表
                    HotelName = co.Room.Hotel.Name,
                    HotelImage = co.Room.Hotel.Image,
                    City = co.Room.Hotel.City,
                    Region = co.Room.Hotel.Region,
                    AvgComment = co.Room.Hotel.AvgComment,
                    HotelPhone = co.Room.Hotel.Phone,

                    //Payments表
                    PaymentType = co.Order.Payments.OrderBy(x => x.PaymentId).Last().Type,

                    //Comments表
                    CommentId = _context.Comments.FirstOrDefault(
                        x => x.HotelId == co.Room.HotelId && x.OrderId == co.OrderId && x.CustomerId == customer.CustomerId).CommentId,
                });
            return customerOrders;
        }

        [HttpGet]
        public IActionResult FilterFinishOrders(int orderId)
        {
            if (orderId == 0)
            {
                return Content("請輸入正確訂單編號!");
            }
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var customerOrders = _context.OrderDetails.Include(x => x.Order).Include(x => x.Room).ThenInclude(x => x.Hotel).Where(
                x => x.Order.CustomerId == customer.CustomerId && x.OrderId == orderId && x.Order.Status == 2 && x.CheckOut < DateTime.Now).Select(co => new CustomerOrdersViewModel
                {   //OrderDetails表
                    OrderId = co.OrderId,
                    OrderDetailId = co.OrderDetailId,
                    RoomId = co.RoomId,
                    CheckIn = co.CheckIn,
                    CheckOut = co.CheckOut,
                    NumberOfGuests = co.NumberOfGuests,
                    TotalPrice = co.RoomTotalPrice + co.EspriceTotal - co.DiscountTotalPrice,
                    Note = co.Note,

                    //Orders表
                    NameOfBooking = co.Order.NameOfBooking,
                    OrderDate = co.Order.Date,
                    Status = co.Order.Status,

                    //Rooms表
                    RoomType = co.Room.Type,
                    HotelId = co.Room.HotelId,

                    //Hotels表
                    HotelName = co.Room.Hotel.Name,
                    HotelImage = co.Room.Hotel.Image,
                    City = co.Room.Hotel.City,
                    Region = co.Room.Hotel.Region,
                    AvgComment = co.Room.Hotel.AvgComment,
                    HotelPhone = co.Room.Hotel.Phone,

                    //Payments表
                    PaymentType = co.Order.Payments.OrderBy(x => x.PaymentId).Last().Type,

                    //Comments表
                    CommentId = _context.Comments.FirstOrDefault(
                        x => x.HotelId == co.Room.HotelId && x.OrderId == co.OrderId && x.CustomerId == customer.CustomerId).CommentId,
                });
            if (!customerOrders.Any())
            {
                return Content("查無此訂單編號或此訂單尚未結束!");
            }
            return Ok(customerOrders);

        }

        [HttpPost]
        public IActionResult GetComment([FromBody] CommentViewModel cv)
        {
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var comment = _context.Comments.AsNoTracking().FirstOrDefault(x => x.CustomerId == customer.CustomerId && x.HotelId == cv.HotelId && x.OrderId == cv.OrderId);
            if (comment != null)
            {
                var cm = new CommentViewModel
                {
                    CommentId = comment.CommentId,
                    HotelId = comment.HotelId,
                    OrderId = comment.OrderId,
                    Cp = comment.Cp,
                    Comfortable = comment.Comfortable,
                    Staff = comment.Staff,
                    Facility = comment.Facility,
                    Clean = comment.Clean,
                    Note = comment.Note,
                    Date = comment.Date,
                };
                return Ok(cm);
            }
            var noCm = new CommentViewModel
            {
                HotelId = cv.HotelId,
                OrderId = cv.OrderId,
                CustomerId= customer.CustomerId,
                Cp = 0,
                Comfortable = 0,
                Staff = 0,
                Facility = 0,
                Clean = 0,
                Note="",
            };
            return Ok(noCm);
        }

        [HttpPost]
        public async Task<string> PostComment([FromBody] CommentViewModel cm)
        {
            if (cm.Cp == 0 || cm.Comfortable == 0 || cm.Staff == 0 || cm.Facility == 0 || cm.Clean == 0)
            {
                return "評分範圍為1顆星至5顆星!";
            }
            if (cm.CommentId != 0)
            {
                Comment existComment = await _context.Comments.FindAsync(cm.CommentId);
                existComment.Cp = cm.Cp;
                existComment.Comfortable = cm.Comfortable;
                existComment.Staff = cm.Staff;
                existComment.Facility = cm.Facility;
                existComment.Clean = cm.Clean;
                existComment.Note = cm.Note;
                existComment.Date = DateTime.Now;

                _context.Entry(existComment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return "已修改住宿評鑑!";
            }
            Comment comment = new Comment
            {   
                HotelId = cm.HotelId,
                OrderId = cm.OrderId,
                CustomerId = cm.CustomerId,
                Cp = cm.Cp,
                Comfortable = cm.Comfortable,
                Staff = cm.Staff,
                Facility = cm.Facility,
                Clean = cm.Clean,
                Note = cm.Note,
                Date = DateTime.Now,
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return "已完成住宿評鑑!";
        }

    }

}

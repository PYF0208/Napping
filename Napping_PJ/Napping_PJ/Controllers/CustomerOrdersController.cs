using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Security.Claims;

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

        [HttpGet]
        public IEnumerable<CustomerOrdersViewModel> GetCustomerOrders()
        {
            var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            var customerOrders = _context.OrderDetails.Include(x => x.Order).Include(x => x.Room).ThenInclude(x => x.Hotel).Where(x => x.Order.CustomerId == customer.CustomerId).Select(co => new CustomerOrdersViewModel
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
                x => x.Order.CustomerId == customer.CustomerId&&x.OrderId==orderId).Select(co => new CustomerOrdersViewModel
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
                Status= co.Order.Status,

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
        public string ShowCheckOut(int status,int orderId)
        {
            if (status == 1) {
                return $"<a href=\"/CheckOut/IndexByOrder?orderId={orderId}\" class=\"btn btn-primary\">跳轉結帳頁面</a>";
            }
            return string.Empty;
        }
    }

}

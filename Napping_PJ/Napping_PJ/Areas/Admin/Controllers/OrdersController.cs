using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public OrdersController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Admin/Orders/Details/5
        [HttpGet]
        public async Task<IEnumerable<OrdersViewModel>> GetOrder()
        {
            var ord = _context.Orders.Include(o => o.OrderDetails).Include(x => x.Customer).Include(x => x.Payment).Select(o => new OrdersViewModel
            {

                CustomerId = o.CustomerId,
                OrderId = o.OrderId,
                PaymentId = o.PaymentId,
                Date = o.Date,
                CustomerName = o.Customer.Name,

                /* 訂單明細*/
                

            }) ;

            return ord;
        }
        [HttpGet]
        public async Task<IEnumerable<PaymentViewModel>> GetPayment()
        {
            var Pa = _context.Payments.Select(o => new PaymentViewModel
            {
                Date = o.Date,
                OrderId = o.OrderId,
                PaymentId = o.PaymentId,
                Status = o.Status,
                Type = o.Type,
            });
            return Pa;
        }
        [HttpGet]
        public async Task<IEnumerable<MemberViewModel>> GetMember()
        {
            var Mem = _context.Customers.Select(c => new MemberViewModel
            {
                Birthday = c.Birthday,
                City = c.City,
                Country = c.Country,
                Email = c.Email,
                Gender = c.Gender,
                LevelId = c.LevelId,
                Phone = c.Phone,
                Region = c.Region,
                Name = c.Name,
                CustomerId = c.CustomerId,
            });
            return Mem;
        }
        [HttpGet]
        public async Task<IEnumerable<OrderDetailsViewModel>> GetOrderDetails()
        {
            var od = _context.OrderDetails.Include(o=>o.Order).Select(od => new OrderDetailsViewModel
            {

                OrderDetailId = od.OrderDetailId,
                RoomId = od.RoomId,
                
                CheckIn = od.CheckIn,
                CheckOut = od.CheckOut,
                NumberOfGuests = od.NumberOfGuests,
                TravelType = od.TravelType,
                Note = od.Note,
                OrderId = od.OrderId,
                Date=od.Order.Date
            });
            return od;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders

                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create


        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<string> Create([FromBody] OrdersViewModel order)
        {
            try
            {
                _context.Orders.Add(new Order()
                {
                    CustomerId = order.CustomerId,
                    PaymentId = order.PaymentId,
                    Date = order.Date,

                });
                _context.SaveChanges();
                return "創建成功";

            }
            catch (Exception)
            {

                return "創建失敗";
            }


        }


        // GET: Admin/Orders/Edit/5


        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public async Task<string> Edit(int id, [FromBody] OrdersViewModel order)
        {
            var ord = await _context.Orders.FindAsync(id);
            if (id != order.OrderId && ord.OrderId != order.OrderId)
            {
                return "修改失敗";
            }
            ord.OrderId = order.OrderId;
            ord.CustomerId = order.CustomerId;
            ord.PaymentId = order.PaymentId;
            ord.Date = order.Date;
            try
            {
                _context.Update(ord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(ord.OrderId))
                {
                    return "修改失敗";
                }
                else
                {
                    throw;
                }
            }
            return "修改成功";
        }



        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpDelete]

        public async Task<string> DeleteConfirmed(int id)
        {
            var or = await _context.Orders.FindAsync(id);
            if (or == null)
            {
                return "刪除失敗";
            }

            if (or != null)
            {
                _context.Orders.Remove(or);
            }

            await _context.SaveChangesAsync();
            return "刪除成功";
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}


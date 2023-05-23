using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Napping_PJ.Areas.Admin.Models;
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
			var ord = _context.Orders.Include(x=>x.Currency).Include(x=>x.Customer).Include(x=>x.Payment).Select(o => new OrdersViewModel
			{
				CurrencyId = o.CurrencyId,
				CustomerId = o.CustomerId,
				Date = o.Date,
				OrderId = o.OrderId,
				PaymentId = o.PaymentId,
			});
			return ord;
		}
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Orders == null)
			{
				return NotFound();
			}

			var order = await _context.Orders
				.Include(o => o.Currency)
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
		public IActionResult Create()
		{
			ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "CurrencyId");
			ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
			ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId");
			return View();
		}

		// POST: Admin/Orders/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]

		public async Task<string> Create([FromBody] OrdersViewModel order)
		{
			var o = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
			if (o != null)
			{
				var Neworder = new Order
				{
					OrderId = order.OrderId,
					CustomerId = order.CustomerId,
					CurrencyId = order.CurrencyId,
					Date = order.Date,
					PaymentId = order.PaymentId,

				};
				_context.Add(Neworder);
				await _context.SaveChangesAsync();
				return "創建成功";
			}
			return "創建失敗";
		}

		// GET: Admin/Orders/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Orders == null)
			{
				return NotFound();
			}

			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				return NotFound();
			}
			ViewData["CurrencyId"] = new SelectList(_context.Currencies, "CurrencyId", "CurrencyId", order.CurrencyId);
			ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
			ViewData["PaymentId"] = new SelectList(_context.Payments, "PaymentId", "PaymentId", order.PaymentId);
			return View(order);
		}

		// POST: Admin/Orders/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]

		public async Task<string> Edit(int id, [FromBody] OrdersViewModel order)
		{
			var ord = await _context.Orders.FindAsync(id);
			if (id != order.OrderId && ord.OrderId!=order.OrderId)
			{
				return "修改失敗";
			}
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
				.Include(o => o.Currency)
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


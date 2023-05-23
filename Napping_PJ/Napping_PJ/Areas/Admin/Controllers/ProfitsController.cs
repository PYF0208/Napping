using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;
using NuGet.Versioning;

namespace Napping_PJ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfitsController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public ProfitsController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        // GET: Admin/Profits
        public async Task<IActionResult> Index()
        {
              return View();
        }
        [HttpGet]
        public async Task<IEnumerable<ProfitViewModel>> GetProfit()
        {

            var Profit =  _context.Profits.Select(Profit=>new ProfitViewModel

			{
                Number = Profit.Number,
                ProfitId = Profit.ProfitId,
                Date = Profit.Date,

            });

			return Profit;
        }
        // GET: Admin/Profits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Profits == null)
            {
                return NotFound();
            }

            var profit = await _context.Profits
                .FirstOrDefaultAsync(m => m.ProfitId == id);
            if (profit == null)
            {
                return NotFound();
            }

            return View(profit);
        }

        // GET: Admin/Profits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Profits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        
        public async Task<string> NewCreate([FromBody]ProfitViewModel profit)
        {

            var NewProfit = new Profit
            {
                ProfitId = profit.ProfitId,
                Date = profit.Date,
                Number = profit.Number,
            };
            if (NewProfit == null)
            {
                return "創建失敗";
            }
				_context.Add(NewProfit);
                await _context.SaveChangesAsync();
            return "創建成功";
        }

        // GET: Admin/Profits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Profits == null)
            {
                return NotFound();
            }

            var profit = await _context.Profits.FindAsync(id);
            if (profit == null)
            {
                return NotFound();
            }
            return View(profit);
        }

        // POST: Admin/Profits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut]

        public async Task<string> Edit(int id, [FromBody] ProfitViewModel profit)
        {

            var Profit = await _context.Profits.FindAsync(id);

            if (id == null || profit.ProfitId == null || id != profit.ProfitId || Profit == null)
            {
                return "修改失敗";
            }

                Profit.Date = profit.Date;
                Profit.Number = profit.Number;
                Profit.ProfitId = profit.ProfitId;
           


			try
                {
                    _context.Update(Profit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfitExists(Profit.ProfitId))
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

        // GET: Admin/Profits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Profits == null)
            {
                return NotFound();
            }

            var profit = await _context.Profits
                .FirstOrDefaultAsync(m => m.ProfitId == id);
            if (profit == null)
            {
                return NotFound();
            }

            return View(profit);
        }

        // POST: Admin/Profits/Delete/5
        [HttpDelete]
        public async Task<string> DeleteConfirmed(int id)
        {
            var del = await _context.Profits.FindAsync(id);
            if (del == null)
            {
                return "刪除失敗";
            }
            
            if (del != null)
            {
                _context.Profits.Remove(del);
            }
            
            await _context.SaveChangesAsync();
            return "刪除成功";
        }

        private bool ProfitExists(int id)
        {
          return (_context.Profits?.Any(e => e.ProfitId == id)).GetValueOrDefault();
        }
    }
}

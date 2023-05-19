using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models.Entity;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfitId,Date,Number")] Profit profit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profit);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfitId,Date,Number")] Profit profit)
        {
            if (id != profit.ProfitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfitExists(profit.ProfitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(profit);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Profits == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.Profits'  is null.");
            }
            var profit = await _context.Profits.FindAsync(id);
            if (profit != null)
            {
                _context.Profits.Remove(profit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfitExists(int id)
        {
          return (_context.Profits?.Any(e => e.ProfitId == id)).GetValueOrDefault();
        }
    }
}

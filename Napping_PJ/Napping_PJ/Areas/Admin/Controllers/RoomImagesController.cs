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
    public class RoomImagesController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public RoomImagesController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        // GET: Admin/RoomImages
        public async Task<IActionResult> Index()
        {
            var db_a989f8_nappingContext = _context.RoomImages.Include(r => r.Room);
            return View(await db_a989f8_nappingContext.ToListAsync());
        }

        // GET: Admin/RoomImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RoomImages == null)
            {
                return NotFound();
            }

            var roomImage = await _context.RoomImages
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.RoomImageId == id);
            if (roomImage == null)
            {
                return NotFound();
            }

            return View(roomImage);
        }

        // GET: Admin/RoomImages/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId");
            return View();
        }

        // POST: Admin/RoomImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomImageId,RoomId,Image")] RoomImage roomImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", roomImage.RoomId);
            return View(roomImage);
        }

        // GET: Admin/RoomImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RoomImages == null)
            {
                return NotFound();
            }

            var roomImage = await _context.RoomImages.FindAsync(id);
            if (roomImage == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", roomImage.RoomId);
            return View(roomImage);
        }

        // POST: Admin/RoomImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomImageId,RoomId,Image")] RoomImage roomImage)
        {
            if (id != roomImage.RoomImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomImageExists(roomImage.RoomImageId))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", roomImage.RoomId);
            return View(roomImage);
        }

        // GET: Admin/RoomImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RoomImages == null)
            {
                return NotFound();
            }

            var roomImage = await _context.RoomImages
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.RoomImageId == id);
            if (roomImage == null)
            {
                return NotFound();
            }

            return View(roomImage);
        }

        // POST: Admin/RoomImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RoomImages == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.RoomImages'  is null.");
            }
            var roomImage = await _context.RoomImages.FindAsync(id);
            if (roomImage != null)
            {
                _context.RoomImages.Remove(roomImage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomImageExists(int id)
        {
          return (_context.RoomImages?.Any(e => e.RoomImageId == id)).GetValueOrDefault();
        }
    }
}

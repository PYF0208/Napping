using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeatureController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public FeatureController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        // GET: Admin/Feature
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult FilterFeature([FromBody] FeatureViewModel featureVM)
        {
            return Json(_context.Features.Select(ft => new FeatureViewModel
            {
                FeatureId = ft.FeatureId,
                Name = ft.Name,
                Image = ft.Image,
            })
                );
        }

        [HttpPut]
		public async Task<string> UpdateFeature(int id, [FromBody]FeatureViewModel featureVM)
		{
			if (id != featureVM.FeatureId)
			{
				return "修改失敗！";
			}

			Feature ft = await _context.Features.FindAsync(featureVM.FeatureId);
			ft.FeatureId = featureVM.FeatureId;
			ft.Name = featureVM.Name;
			ft.Image = featureVM.Image;
			_context.Entry(ft).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!FeatureExists(id))
				{
					return "修改失敗！";
				}
				else
				{
					throw;
				}
			}

			return "修改成功！";
		}

		[HttpPost]
		public async Task<string> InsertFeature([FromBody] FeatureViewModel featureVM)
		{
			Feature ft = new Feature
			{
				FeatureId = featureVM.FeatureId,
                Name = featureVM.Name,
                Image = featureVM.Image

			};
			_context.Features.Add(ft);
			await _context.SaveChangesAsync();

			return "新增成功！";
		}

		[HttpDelete]
		public async Task<string> DeleteFeature(int id)
		{

			var feature = await _context.Features.FindAsync(id);
			if (feature == null)
			{
				return "刪除失敗";
			}

			_context.Features.Remove(feature);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch
			{
				return "刪除關聯紀錄失敗!";
			}

			return "刪除成功";
		}


		// GET: Admin/Feature/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Features == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FeatureId == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // GET: Admin/Feature/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Feature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeatureId,name,Image")] Feature feature)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feature);
        }

        // GET: Admin/Feature/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Features == null)
            {
                return NotFound();
            }

            var feature = await _context.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }
            return View(feature);
        }

        // POST: Admin/Feature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeatureId,name,Image")] Feature feature)
        {
            if (id != feature.FeatureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureExists(feature.FeatureId))
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
            return View(feature);
        }

        // GET: Admin/Feature/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Features == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FeatureId == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // POST: Admin/Feature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Features == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.Features'  is null.");
            }
            var feature = await _context.Features.FindAsync(id);
            if (feature != null)
            {
                _context.Features.Remove(feature);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureExists(int id)
        {
          return (_context.Features?.Any(e => e.FeatureId == id)).GetValueOrDefault();
        }
    }
}

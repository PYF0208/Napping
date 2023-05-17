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
    public class PromotionsController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public PromotionsController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        // GET: Admin/Promotions
        public async Task<IEnumerable<PromotionViewModel>> GetPromotionService()
        {
            var PromotionService = _context.Promotions.Select(PromotionService => new PromotionViewModel
            {
                PromotionId = PromotionService.PromotionId,
                LevelId = PromotionService.LevelId,
                Name = PromotionService.Name,
                StartDate = PromotionService.StartDate,
                EndDate = PromotionService.EndDate,
                Discount = PromotionService.Discount
            });
            return PromotionService;
        }
        [HttpPost]
        public async Task<IEnumerable<PromotionViewModel>> FilterPromotions(
            [FromBody] PromotionViewModel prViewModel)
        {
            return _context.Promotions.Select(pro => new PromotionViewModel
            {
                PromotionId = pro.PromotionId,
                LevelId = pro.LevelId,
                Name = pro.Name,
                StartDate = pro.StartDate,
                EndDate = pro.EndDate,
                Discount = pro.Discount
            });
        }

           

        [HttpPut("{id}")]
        public async Task<string> PutPromotions(int id, [FromBody] PromotionViewModel promotion)
        {
            if (id != promotion.PromotionId)
            {
                return "修改促銷記錄失敗!";
            }
            Promotion pro = await _context.Promotions.FindAsync(id);
            pro.PromotionId = promotion.PromotionId;
            pro.LevelId = promotion.LevelId;
            pro.Name = promotion.Name;
            pro.StartDate = promotion.StartDate;
            pro.EndDate = promotion.EndDate;
            pro.Discount = promotion.Discount;
            _context.Entry(pro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromotionExists(id))
                {
                    return "修改促銷記錄失敗!";
                }
                else
                {
                    throw;
                }
            }

            return "修改促銷記錄成功!";
        }


        // POST: Admin/Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<string> PostPromotion([FromBody] PromotionViewModel promotion)
        {
            Promotion pro = new Promotion
            {

                PromotionId = promotion.PromotionId,
                LevelId = promotion.LevelId,
                Name = promotion.Name,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Discount = promotion.Discount,
                
            };
            _context.Promotions.Add(pro);
            await _context.SaveChangesAsync();

            return $"促銷編號:{pro.PromotionId}";
        }



        // Delete: Admin/Promotions/Delete/5
        [HttpDelete("{id}")]
            public async Task<string> DeletePromotions(int id)
            {
                var promotion = await _context.Promotions.FindAsync(id);
                if (promotion == null)
                {
                    return "刪除促銷記錄成功!";
                }

                _context.Promotions.Remove(promotion);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return "刪除促銷關聯記錄失敗!";
                }

                return "刪除促銷記錄成功!";
            }

            private bool PromotionExists(int id)
            {
                return (_context.Promotions?.Any(e => e.PromotionId == id)).GetValueOrDefault();
            }
        }
    }

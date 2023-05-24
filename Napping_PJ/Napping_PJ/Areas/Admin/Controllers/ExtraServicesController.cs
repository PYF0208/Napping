﻿using System;
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
    public class ExtraServicesController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public ExtraServicesController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IEnumerable<ExtraServiceViewModel>> GetExtraServices()
        {
            var esViewModel = await _context.ExtraServices
                //.Include(e => e.Hotel)
                .Select(es => new ExtraServiceViewModel
                {
                    ExtraServiceId = es.ExtraServiceId,
                    HotelId = es.HotelId,
                    ExtraServiceName = es.Name,
                    Price = es.Price,
                    HotelName = es.Hotel.Name
                })
                .ToListAsync();

            return esViewModel;
        }

        [HttpGet]
        public async Task<IEnumerable<HotelsViewModel>> GetHotels()
        {
            var hotelsViewModel = await _context.Hotels
                .Select(h => new HotelsViewModel
                {
                    HotelId = h.HotelId,
                    Name = h.Name,
                    Star = h.Star,
                    Image = h.Image,
                    ContactName = h.ContactName,
                    Phone = h.Phone,
                    Email = h.Email,
                    City = h.City,
                    Region = h.Region,
                    Address = h.Address,
                    AvgComment = h.AvgComment,
                })
                .ToListAsync();

            return hotelsViewModel;
        }
        [HttpPost]
        public async Task<IEnumerable<ExtraServiceViewModel>> FilterExtraServices(
            [FromBody] ExtraServiceViewModel esViewModel)
        {
            return await _context.ExtraServices.Where(es =>
            es.ExtraServiceId == esViewModel.ExtraServiceId ||
            es.Price == esViewModel.Price ||
            es.Name.Contains(esViewModel.ExtraServiceName) ||
            es.Hotel.Name.Contains(esViewModel.HotelName)).Select(es => new ExtraServiceViewModel
            {
                ExtraServiceId = es.ExtraServiceId,
                HotelId = es.HotelId,
                ExtraServiceName = es.Name,
                Price = es.Price,
                HotelName = es.Hotel.Name
            }).ToListAsync();

        }

        [HttpPut]
        public async Task<string> PutExtraServices(int id, [FromBody] ExtraServiceViewModel esViewModel)
        {
            if (id != esViewModel.ExtraServiceId)
            {
                return "修改加購服務選項失敗!";
            }
            ExtraService es = await _context.ExtraServices.FindAsync(id);
            es.ExtraServiceId = esViewModel.ExtraServiceId;
            es.Name = esViewModel.ExtraServiceName;
            es.Price = esViewModel.Price;
            es.HotelId = esViewModel.HotelId;
            _context.Entry(es).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtraServiceExists(id))
                {
                    return "修改加購服務選項失敗!";
                }
                else
                {
                    throw;
                }
            }

            return "修改加購服務選項成功!";
        }
        [HttpPost]
        public async Task<string> PostExtraServices([FromBody] ExtraServiceViewModel esViewModel)
        {
            if (esViewModel == null)
            {
                return "欄位不可為空值!";
            }
            ExtraService es = new ExtraService
            {
                ExtraServiceId = esViewModel.ExtraServiceId,
                Name = esViewModel.ExtraServiceName,
                Price = esViewModel.Price,
                HotelId = esViewModel.HotelId

            };
            _context.ExtraServices.Add(es);
            await _context.SaveChangesAsync();

            return $"加購服務編號:{es.ExtraServiceId}";
        }

        [HttpDelete]
        public async Task<string> DeleteExtraServices(int id)
        {

            var extraServices = await _context.ExtraServices.FindAsync(id);
            if (extraServices == null)
            {
                return "刪除加購服務選項成功!";
            }

            _context.ExtraServices.Remove(extraServices);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return "刪除加購服務選項失敗!";
            }

            return "刪除加購服務選項成功!";
        }

        private bool ExtraServiceExists(int id)
        {
            return (_context.ExtraServices?.Any(e => e.ExtraServiceId == id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;
using System.Drawing.Printing;
using System.Text.Json;

namespace Napping_PJ.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public CustomersController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            int pageSize = 5;
            int total = _context.Customers.Count();
            int maxPage = (int)Math.Ceiling((decimal)_context.Customers.Count() / (decimal)pageSize);
            ViewBag.maxPage = maxPage;
            ViewBag.total = total;
            return View();
        }
        public async Task<IActionResult> SkipPage(int pageIndex = 1, string filterKeyword = "")
        {
            filterKeyword = filterKeyword ?? "";
            int pageSize = 5;
            int pageNumber = pageIndex;
            List<CustomersViewModel> customersViewModels = new List<CustomersViewModel>();
            await _context.Customers.Where(c => c.Email.Contains(filterKeyword) || c.City.Contains(filterKeyword) || c.Country.Contains(filterKeyword) || c.Name.Contains(filterKeyword) || c.Phone.Contains(filterKeyword) || c.Region.Contains(filterKeyword)).Skip((pageNumber - 1) * pageSize).Take(pageSize).ForEachAsync(x =>
            {
                customersViewModels.Add(new CustomersViewModel()
                {
                    birthday = x.Birthday,
                    city = x.City,
                    country = x.Country,
                    customerId = x.CustomerId,
                    email = x.Email,
                    gender = x.Gender,
                    levelId = x.LevelId,
                    locked = x.Locked,
                    name = x.Name,
                    phone = x.Phone,
                    region = x.Region,
                });
            });
             return Ok(customersViewModels);
        }
        public async Task<IActionResult> GetLevealList()
        {
            List<Level> levelList = new List<Level>();
            await _context.Levels.ForEachAsync(l =>
            {
                levelList.Add(new Level()
                {
                    levelId = l.LevelId,
                    name = l.Name,
                });
            });
            return new JsonResult(levelList);
        }
        public class Level
        {
            public int levelId { get; set; }
            public string name { get; set; }

        }
        public async Task<IActionResult> RemoveCuctomer([FromBody] CustomersViewModel customersViewModel)
        {
            Customer getCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customersViewModel.email);
            if (getCustomer == null)
            {
                return BadRequest();
            }
            try
            {
                _context.Customers.Remove(getCustomer);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex);
            }
            return Ok();
        }
        public async Task<IActionResult> EditCustomer([FromBody] CustomersViewModel customersViewModel)
        {

            Customer getCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customersViewModel.email);
            if (getCustomer == null)
            {
                return BadRequest();
            }
            getCustomer.Name = customersViewModel.name;
            getCustomer.Birthday = customersViewModel.birthday;
            getCustomer.Phone = customersViewModel.phone;
            getCustomer.Gender = customersViewModel.gender;
            getCustomer.City = customersViewModel.city;
            getCustomer.Region = customersViewModel.region;
            getCustomer.Country = customersViewModel.country;
            getCustomer.Email = customersViewModel.email;
            getCustomer.LevelId = customersViewModel.levelId;
            getCustomer.Locked = customersViewModel.locked;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomersViewModel customersViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Customer getCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customersViewModel.email);
            if (getCustomer != null)
            {
                return BadRequest();
            }
            Customer newCustomer = new Customer();
            newCustomer.Name = customersViewModel.name;
            newCustomer.Birthday = customersViewModel.birthday;
            newCustomer.Phone = customersViewModel.phone;
            newCustomer.Gender = customersViewModel.gender;
            newCustomer.City = customersViewModel.city;
            newCustomer.Region = customersViewModel.region;
            newCustomer.Country = customersViewModel.country;
            newCustomer.Email = customersViewModel.email;
            newCustomer.LevelId = customersViewModel.levelId;
            newCustomer.Locked = customersViewModel.locked;
            try
            {
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ValidFileds([FromBody] CustomersViewModel customersViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(ModelState);
        }
    }
}

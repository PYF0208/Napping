using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Security.Claims;

namespace Napping_PJ.Controllers
{
    [Route("api/Result/[action]")]
    [ApiController]
    public class ResultApiController : ControllerBase
    {
        private readonly db_a989f8_nappingContext _context;

        public ResultApiController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ResultViewModel> Get()
        {
            HashSet<int> allHotel = null;
            if (User.Identity.IsAuthenticated)
            {
                var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
                allHotel = _context.Likes.AsNoTracking().Where(x => x.CustomerId == customer.CustomerId).Select(x => x.HotelId).ToHashSet();
            }
            return _context.Hotels.AsNoTracking().Join(_context.Rooms, h => h.HotelId, r => r.HotelId, (h, r) => new ResultViewModel
            {

                HotelId = h.HotelId,
                Name = h.Name,
                Image = h.Image,
                City = h.City,
                Region = h.Region,
                Price = r.Price,
                MaxGuests = r.MaxGuests,
                PositionLat = (float)h.PositionLat,
                PositionLon = (float)h.PositionLon,
                IsLike = (allHotel != null && allHotel.Contains(h.HotelId)) ? true : false,
            });
        }
    }
}

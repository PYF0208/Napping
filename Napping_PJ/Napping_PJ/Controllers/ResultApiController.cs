using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;

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
        public IActionResult Get()
        {

            return Ok(_context.Hotels.Select(x => new ResultViewModel
            {
                HotelId = x.HotelId,
                Name = x.Name,
                Image = x.Image,
                City = x.City,
                Region = x.Region,

            }));

        }




    };
}

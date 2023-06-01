using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models;
using Napping_PJ.Models.Entity;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace Napping_PJ.Controllers
{
    public class CartController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public CartController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SetSession([FromBody] IEnumerable<RoomDetailViewModel> roomDetailViewModels)
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            Customer loginUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmailClaim.Value);
            //string userEmail = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            byte[] bytes = null;
            string json = JsonConvert.SerializeObject(roomDetailViewModels);
            bytes = System.Text.Encoding.UTF8.GetBytes(json);
            HttpContext.Session.Set($"{loginUser.CustomerId}_cartItem", bytes);
            return Ok();
        }

        public async Task<IActionResult> GetSession()
        {
            Claim userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim != null)
            {
                Customer loginUser = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmailClaim.Value);

                byte[] bytes = HttpContext.Session.Get($"{loginUser.CustomerId}_cartItem");
                if (bytes == null)
                {
                    return BadRequest("購物車為空");
                }
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                IEnumerable<RoomDetailViewModel> roomDetailViewModels = JsonConvert.DeserializeObject<IEnumerable<RoomDetailViewModel>>(json);
                return Ok(roomDetailViewModels);
            }

            return BadRequest("尚未登入");
        }
    }
}

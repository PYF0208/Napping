using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol.Plugins;
using System.ComponentModel;

namespace Napping_PJ.Controllers
{

    public class LikeController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public LikeController(db_a989f8_nappingContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetLikes()
        {
            HashSet<int> allHotel = null;
            if (User.Identity.IsAuthenticated)
            {
                var customer = _context.Customers.AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
                allHotel = _context.Likes.AsNoTracking().Where(x => x.CustomerId == customer.CustomerId).Select(x => x.HotelId).ToHashSet();
                var likeViewModel = _context.Likes.Include(x=>x.Hotel).ThenInclude(x=>x.Rooms).Where(Likes => Likes.CustomerId == customer.CustomerId).Select(Likes => new LikeViewModel
                {
                    HotelId = Likes.Hotel.HotelId,
                    HotelName = Likes.Hotel.Name,
                    City = Likes.Hotel.City,
                    Region = Likes.Hotel.Region,
                    HotelImage = Likes.Hotel.Image,
                    LowestPrice = Likes.Hotel.Rooms.First().Price,
                    CreateDate = Likes.CreateDate,
                    IsLike = (allHotel != null && allHotel.Contains(Likes.HotelId)) ? true : false,
                });
                return Ok(likeViewModel);
            }
            return BadRequest("/Login/Index");

        }
        [HttpGet]
        public IActionResult ClickLike(int hotelId)
        {
            if (User.Identity == null)
            {
				return BadRequest("/Login/Index");
			}
            if (!User.Identity.IsAuthenticated)
            {
				return BadRequest("/Login/Index");
			}
            var user = _context.Customers.Include(x => x.Likes).AsNoTracking().FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            if (user == null)
            {
				return BadRequest("/Login/Index");
			}
            var findResult = user.Likes.FirstOrDefault(x => x.HotelId == hotelId);
            if (findResult != null)
            {
                _context.Likes.Remove(findResult);
            }
            else
            {
                _context.Likes.Add(new Like()
                {
                    HotelId = hotelId,
                    CustomerId = user.CustomerId,
                    CreateDate = DateTime.Now
                });
            }
            _context.SaveChanges();
            return Ok(new ApiResponseDto
            {
                Code = "A103",
                Message = "修改成功"
            });


            #region 原版本

            //if (User.Identity.IsAuthenticated)
            //{
            //    var customer = _context.Customers.FirstOrDefault(x => x.Email == User.FindFirst(ClaimTypes.Email).Value);
            //    int FindCustomer = customer.CustomerId;
            //    var FindLike = _context.Likes.FirstOrDefault(x => x.CustomerId == FindCustomer && x.HotelId == hotelId);

            //    if (FindLike == null)
            //    {
            //        _context.Likes.Add(new Like()
            //        {
            //            HotelId = hotelId,
            //            CustomerId = FindCustomer,
            //            CreateDate = DateTime.Now
            //        });
            //        _context.SaveChanges();

            //    }
            //    else
            //    {
            //        _context.Likes.Remove(FindLike);
            //        _context.SaveChanges();
            //    }
            //    return Ok();
            //}
            //return NotFound(); 
            #endregion
        }


    }
}

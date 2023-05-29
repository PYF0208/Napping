using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Napping_PJ.Areas.Admin.Models;
using Napping_PJ.Models.Entity;
using Newtonsoft.Json;

namespace Napping_PJ.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly db_a989f8_nappingContext _context;

        public RolesController(db_a989f8_nappingContext context)
        {
            _context = context;
        }
        
        //[Authorize(Roles = "1")]
        //[Authorize(Roles = "3")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetUserRoleList()
        {
            List<UserRoleViewModel> allUserRoleViewModel = new List<UserRoleViewModel>();

            Customer[] cusList = await _context.Customers.ToArrayAsync();
            UserRole[] userRoleList = await _context.UserRoles.ToArrayAsync();
            IEnumerable<Role> roleList = _context.Roles;

            foreach (Customer cs in cusList)
            {
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel()
                {
                    customer = cs,
                    SelectedRole = new List<Role>(),
                };
                foreach (var userRole in userRoleList.Where(ur => ur.CustomerId == cs.CustomerId))
                {
                    userRoleViewModel.SelectedRole.Add(roleList.First(r => r.RoleId == userRole.RoleId));
                }
                allUserRoleViewModel.Add(userRoleViewModel);
            }

            RolesIndexViewModel rolesIndexViewModel = new RolesIndexViewModel()
            {
                UserRoleViewModels = allUserRoleViewModel,
                Roles = roleList
            };

            string json = JsonConvert.SerializeObject(rolesIndexViewModel, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Ok(json);
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddRoleToUser([FromQuery] string userId, [FromQuery] string roleId)
        {
            var user = await _context.Customers.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return BadRequest("無此使用者");
            }
            var role = await _context.Roles.FindAsync(Convert.ToInt32(roleId));
            if (role == null)
            {
                return BadRequest("請選擇正確角色");
            }
            UserRole userRole = new UserRole()
            {
                RoleId = role.RoleId,
                CustomerId = user.CustomerId,
                Customer = user,
                Role = role,
            };
            bool hasRole = await _context.UserRoles.AnyAsync(ur => ur.RoleId == userRole.RoleId && ur.CustomerId == userRole.CustomerId);
            if (hasRole)
            {
                return BadRequest("已有此權限");
            }
            await _context.UserRoles.AddAsync(userRole);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("新增權限成功");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRoleToUser([FromQuery] string userId, [FromQuery] string roleId)
        {
            var user = await _context.Customers.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return BadRequest("無此使用者");
            }
            var role = await _context.Roles.FindAsync(Convert.ToInt32(roleId));
            if (role == null)
            {
                return BadRequest("無此角色");
            }

            UserRole getRole = await _context.UserRoles.FirstAsync(ur => ur.RoleId == role.RoleId && ur.CustomerId == user.CustomerId);
            if (getRole == null)
            {
                return BadRequest("無此權限");
            }
            _context.UserRoles.Remove(getRole);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("刪除成功");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModel roleViewModel)
        {
            Role getRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleViewModel.RoleId);
            if(getRole == null)
            {
                return BadRequest("無此Role");
            }
            try
            {
                getRole.Name = roleViewModel.Name;
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("更新失敗");
            }
            return Ok("更新成功");
        }
        
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody][Bind("RoleId,Name")] RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                Role role = new Role()
                {
                    RoleId = roleViewModel.RoleId,
                    Name = roleViewModel.Name,
                };
                _context.Add(role);
                await _context.SaveChangesAsync();
                return Ok("新增成功");
            }
            return BadRequest("新增失敗");
        }

        // POST: Admin/Roles/RemoveRole?roleId=5
        [HttpPost]
        public async Task<IActionResult> RemoveRole(int roleId)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return Ok("刪除成功");
            }
            return BadRequest("刪除失敗");
        }
    }
}

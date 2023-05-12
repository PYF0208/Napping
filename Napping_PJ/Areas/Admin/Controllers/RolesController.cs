using System;
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

        [Authorize(Roles = "1")]
        //[Authorize(Roles = "3")]
        // GET: Admin/Roles
        public async Task<IActionResult> Index()
        {

            List<UserRoleViewModel> allUserRoleViewModel = new List<UserRoleViewModel>();

            Customer[] cusList = await _context.Customers.ToArrayAsync();
            UserRole[] userRoleList = await _context.UserRoles.ToArrayAsync();
            Role[] roleList = await _context.Roles.ToArrayAsync();
            foreach (Customer cs in cusList)
            {
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel()
                {
                    customer = cs,
                    SelectedRole = new List<Role>(),
                    //AllRoleList = new List<Role>()
                };
                foreach (var userRole in userRoleList.Where(ur => ur.CustomerId == cs.CustomerId))
                {
                    userRoleViewModel.SelectedRole.Add(roleList.First(r => r.RoleId == userRole.RoleId));
                }
                ViewBag.Roles = new SelectList(roleList, "RoleId", "Name");
                allUserRoleViewModel.Add(userRoleViewModel);
            }
            return _context.Roles != null ?
                        View(allUserRoleViewModel) :
                        Problem("Entity set 'db_a989f8_nappingContext.Roles'  is null.");
        }
        [HttpPost]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleId)
        {
            var user = await _context.Customers.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return NotFound();
            }
            var role = await _context.Roles.FindAsync(Convert.ToInt32(roleId));
            if (role == null)
            {
                return NotFound();
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

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveRoleToUser(string userId, string roleId)
        {
            var user = await _context.Customers.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return NotFound();
            }
            var role = await _context.Roles.FindAsync(Convert.ToInt32(roleId));
            if (role == null)
            {
                return NotFound();
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

            return RedirectToAction("Index");
        }

        // GET: Admin/Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Admin/Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Admin/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleName")] Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleId))
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
            return View(role);
        }

        // GET: Admin/Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id)
        {
            return (_context.Roles?.Any(e => e.RoleId == id)).GetValueOrDefault();
        }
    }
}

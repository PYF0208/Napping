using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Napping_API.Helpers;
using Napping_API.Models;
using Napping_API.Models.Entity;
using NuGet.Common;

namespace Napping_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegidterDTOesController : ControllerBase
    {
        private readonly db_a989f8_nappingContext _context;

        public RegidterDTOesController(db_a989f8_nappingContext context)
        {
            _context = context;
        }

        // GET: api/RegidterDTOes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegidterDTO>>> GetRegidterDTO()
        {
            if (_context.RegidterDTO == null)
            {
                return NotFound();
            }
            return await _context.RegidterDTO.ToListAsync();
        }

        // POST: api/RegidterDTOes/TryRegister
        [HttpPost("TryRegister")]
        public async Task<IActionResult> GetRegidterDTO([FromBody] RegidterDTO regidterRequest)
        {
            //確認Email是否已存在
            bool emailExist = await _context.Customers.AnyAsync(c => c.Email == regidterRequest.Email);
            if (emailExist)
            {
                var errorMessage = new
                {
                    errorMessage = "信箱已存在"
                };
                return BadRequest(errorMessage);
            }
            //驗證資料是否符合RegidterDTO規範
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            // 创建新用户
            var newUser = new Customer
            {
                Name = regidterRequest.Email,
                Email = regidterRequest.Email,
                //密碼加鹽加密
                Password = PasswordHasher.HashPassword(regidterRequest.Password, regidterRequest.Email)
            };
            try
            {
                await _context.Customers.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = new
                {
                    errorMessage = ex.InnerException?.Message
                };
                // 處理錯誤訊息...
                return BadRequest(errorMessage);
            }
            // 添加成功，可以獲取新的 ID
            int getCustomerId = newUser.CustomerId;

            // 添加默認角色到用戶角色表
            UserRole newUserRole = new UserRole()
            {
                RoleId = 3,
                CustomerId = getCustomerId
            };
            try
            {
                await _context.UserRoles.AddAsync(newUserRole);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 處理錯誤訊息...
                var errorMessage = new
                {
                    errorMessage = ex.InnerException?.Message
                };
                return BadRequest(errorMessage);
            }
            int getUserRole = newUserRole.RoleId;
            // 创建JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.CustomerId.ToString()),
                new Claim(ClaimTypes.Name, newUser.Name),
                new Claim(ClaimTypes.Role, getUserRole.ToString())
            };
            var expires = DateTime.Now.AddMinutes(30);
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                //_configuration["Jwt:Issuer"],
                //_configuration["Jwt:Issuer"],
                claims: claims,
                expires: expires
                //signingCredentials: creds
                );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // PUT: api/RegidterDTOes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegidterDTO(string id, RegidterDTO regidterDTO)
        {
            if (id != regidterDTO.Email)
            {
                return BadRequest();
            }

            _context.Entry(regidterDTO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegidterDTOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RegidterDTOes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegidterDTO>> PostRegidterDTO(RegidterDTO regidterDTO)
        {
            if (_context.RegidterDTO == null)
            {
                return Problem("Entity set 'db_a989f8_nappingContext.RegidterDTO'  is null.");
            }
            _context.RegidterDTO.Add(regidterDTO);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RegidterDTOExists(regidterDTO.Email))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRegidterDTO", new { id = regidterDTO.Email }, regidterDTO);
        }

        // DELETE: api/RegidterDTOes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegidterDTO(string id)
        {
            if (_context.RegidterDTO == null)
            {
                return NotFound();
            }
            var regidterDTO = await _context.RegidterDTO.FindAsync(id);
            if (regidterDTO == null)
            {
                return NotFound();
            }

            _context.RegidterDTO.Remove(regidterDTO);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegidterDTOExists(string id)
        {
            return (_context.RegidterDTO?.Any(e => e.Email == id)).GetValueOrDefault();
        }
    }
}

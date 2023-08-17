using HR_API.Data;
using HR_API.DTOs;
using HR_API.Models;
using HR_API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly HRAPIDbContext dbContext;

        public RoleController(HRAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Get a list of all the user roles.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(
                dbContext.Roles
                    .Include(r => r.Users)
                    .Select(
                        r =>
                            new
                            {
                                r.Id,
                                r.Name,
                                Users = r.Users.Select(
                                    u =>
                                        new
                                        {
                                            u.Id,
                                            u.Name,
                                            u.LastName,
                                            u.Email,
                                            u.PersonalAddress,
                                            u.Phone,
                                            u.WorkingStartDate
                                        }
                                )
                            }
                    )
                    .ToList()
            );
        }
        /// <summary>
        /// Get specific role by ID
        /// </summary>
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetRoleById([FromRoute] Guid id)
        {
            var role = dbContext.Roles
                .Where(r => r.Id == id)
                .Include(u => u.Users)
                .Select(
                    r =>
                        new
                        {
                            r.Id,
                            r.Name,
                            Users = r.Users.Select(
                                u =>
                                    new
                                    {
                                        u.Id,
                                        u.Name,
                                        u.LastName,
                                        u.Email,
                                        u.PersonalAddress,
                                        u.Phone,
                                        u.WorkingStartDate
                                    }
                            )
                        }
                )
                .FirstOrDefault();
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        /// <summary>
        /// Add New Role
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] AddRoleRequest request)
        {
            var roleName = request.Name;
            var validRoles = Enum.GetNames(typeof(RoleType));

            if (!validRoles.Contains(request.Name))
            {
                return BadRequest($"{request.Name} it not a valid Role");
            }

            var existedRole = dbContext.Roles.Where(r => r.Name == roleName).FirstOrDefault();

            if (existedRole == null)
            {
                var role = new Role { Name = roleName };
                dbContext.Roles.Add(role);
                dbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest($"Role {request.Name} aldready exist");
            }
        }

        /// <summary>
        /// Delete role by ID
        /// </summary>
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var role = await dbContext.Roles.FindAsync(id);
            if (role != null)
            {
                dbContext.Remove(role);
                await dbContext.SaveChangesAsync();
                return NoContent();
            }

            return NotFound($"User with id {id} not exist");
        }
    }
}

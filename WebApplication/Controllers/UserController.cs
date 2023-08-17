using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HR_API.Data;
using Microsoft.EntityFrameworkCore;
using HR_API.Models;
using HR_API.DTOs;
using HR_API.Utils;
using System.Linq.Expressions;
using System.Data;
using System;

namespace HR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HRAPIDbContext dbContext;

        public UserController(HRAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Get all a list of all the workers in the company, including their roles.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(
                dbContext.Users
                    .Include(u => u.Roles)
                    .Select(
                        u =>
                            new
                            {
                                u.Id,
                                u.Name,
                                u.LastName,
                                u.Email,
                                u.PersonalAddress,
                                u.Phone,
                                u.WorkingStartDate,
                                Roles = u.Roles.Select(r => new { r.Id, r.Name })
                            }
                    )
                    .ToList()
            );
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetUserBydId([FromRoute] Guid id)
        {
            var user = dbContext.Users
                .Where(u => u.Id == id)
                .Include(u => u.Roles)
                .Select(
                    u =>
                        new
                        {
                            u.Id,
                            u.Name,
                            u.LastName,
                            u.Email,
                            u.PersonalAddress,
                            u.Phone,
                            u.WorkingStartDate,
                            Roles = u.Roles.Select(r => new { u.Id, u.Name })
                        }
                )
                .FirstOrDefault();
            if (user == null)
            {
                return NotFound($"User with id {id} not exist");
            }
            return Ok(user);
        }

        /// <summary>
        /// For a given user, the list of the salaries increases for each period, including 
        /// the starting salary, and the dates for each increase.
        /// </summary>
        [HttpGet]
        [Route("/historical-salaries/{id:guid}")]
        public async Task<IActionResult> GetHisoricalSalariesByUser([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userSalary = dbContext.Salaries.Where(s => s.User == user).FirstOrDefault();

            if (userSalary == null)
            {
                return NotFound();
            }

            var reports = dbContext.SalaryReports
                .Where(r => r.Salary == userSalary)
                .OrderBy(r => r.ReportDate)
                .Select(
                    r =>
                        new
                        {
                            r.Id,
                            r.ReportDate,
                            r.Balance
                        }
                )
                .ToList();

            return Ok(reports);
        }
        /// <summary>
        /// Create a New User
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            if (request.Roles.Length == 0)
            {
                return BadRequest("Almost one role is neccesary to associate to a user");
            }
            else
            {
                var roles = new List<Role>();

                foreach (var roleName in request.Roles)
                {
                    var validRoles = Enum.GetNames(typeof(RoleType));

                    if (!validRoles.Contains(roleName))
                    {
                        return BadRequest($"{roleName} it not a valid Role");
                    }

                    var existedRole = dbContext.Roles
                        .Where(r => r.Name == roleName)
                        .FirstOrDefault();

                    if (existedRole == null)
                    {
                        var role = new Role() { Name = roleName };
                        roles.Add(role);

                        dbContext.Roles.Add(role);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        roles.Add(existedRole);
                    }
                }

                var user = new User()
                {
                    Name = request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    PersonalAddress = request.PersonalAddress,
                    Phone = request.Phone,
                    Roles = roles
                };

                dbContext.Users.Add(user);

                var result = dbContext.Users.Add(user);

                var salary = new Salary() { CurrentBalance = request.StartedSalary, User = user };

                var report = new SalaryReport()
                {
                    Balance = request.StartedSalary,
                    Salary = salary
                };
                dbContext.Salaries.Add(salary);
                dbContext.SalaryReports.Add(report);
                dbContext.SaveChanges();
                return Ok(new { result.Entity.Id });
            }
        }

        /// <summary>
        /// Request a salary revision for a user.
        /// </summary>
        [HttpPost]
        [Route("/salary-revision/{id:guid}")]
        public async Task<IActionResult> RequestSalaryRevision([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            await dbContext.Entry(user).Collection(u => u.Roles).LoadAsync();
            var roles = user.Roles.Select(x => x.Name).ToList();
            var userSalary = dbContext.Salaries.Where(s => s.User == user).FirstOrDefault();

            if (userSalary == null || roles == null)
            {
                return NotFound();
            }
            var endDate = DateTime.UtcNow;
            var startDate = userSalary.LastBalanceChangeDate;
            int monthsDifference = Tools.GetMonthsDifference(startDate, endDate);

            if (monthsDifference < 3)
            {
                return BadRequest("The salary revision should be performed every 3 months only");
            }
            var increase = Tools.IncreaseByRole(roles) * userSalary.CurrentBalance;

            userSalary.CurrentBalance += increase;
            userSalary.LastBalanceChangeDate = DateTime.UtcNow;

            dbContext.SalaryReports.Add(
                new SalaryReport() { Balance = userSalary.CurrentBalance, Salary = userSalary }
            );
            dbContext.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Update different field of specific user by ID
        /// </summary>
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateUser(
            [FromRoute] Guid id,
            [FromBody] UpdateUserRequest request
        )
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (request.Name != null)
            {
                user.Name = request.Name;
            }

            if (request.LastName != null)
            {
                user.LastName = request.LastName;
            }

            if (request.Email != null)
            {
                user.Email = request.Email;
            }

            if (request.PersonalAddress != null)
            {
                user.PersonalAddress = request.PersonalAddress;
            }

            if (request.Phone != null)
            {
                user.Phone = request.Phone;
            }

            if (request.Roles != null && request.Roles.Length > 0)
            {
                var roles = new List<Role>();

                foreach (var roleName in request.Roles)
                {
                    var validRoles = Enum.GetNames(typeof(RoleType));

                    if (!validRoles.Contains(roleName))
                    {
                        return BadRequest($"{roleName} it not a valid Role");
                    }

                    var existedRole = dbContext.Roles
                        .Where(r => r.Name == roleName)
                        .FirstOrDefault();

                    if (existedRole == null)
                    {
                        var role = new Role() { Name = roleName };
                        roles.Add(role);

                        dbContext.Roles.Add(role);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        roles.Add(existedRole);
                    }
                }

                user.Roles = roles;
            }

            await dbContext.SaveChangesAsync();
            return Ok(user);
        }
        /// <summary>
        /// Remove a user by Id
        /// </summary>
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> RemoveUserById([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                dbContext.Remove(user);
                await dbContext.SaveChangesAsync();
                return NoContent();
            }

            return NotFound($"User with id {id} not exist");
        }
    }
}

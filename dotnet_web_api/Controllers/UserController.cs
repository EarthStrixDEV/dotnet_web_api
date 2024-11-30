using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dotnet_web_api.Database;
using dotnet_web_api.Services;
using dotnet_web_api.Model;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace dotnet_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ValidatorService _validatorService;

        public UserController(ApplicationDbContext context, ValidatorService validatorService)
        {
            _context = context;
            _validatorService = validatorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();

            if (user is null && user?.Count == 0)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound(new { message = $"User with ID {id} not Found." });
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (String.IsNullOrEmpty(user.user_name) && String.IsNullOrEmpty(user.user_email) && String.IsNullOrEmpty(user.user_password))
            {
                return BadRequest(new { message = "Name ,Email and Password are required." });
            }

            if (!_validatorService.IsEmailFormat(user.user_email))
            {
                return BadRequest(new { message = "Email is invalid format." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, User NewUser)
        {
            if (id != NewUser.Id)
            {
                return BadRequest(new { message = "ID in URL doesn't not match ID in body." });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync($"{id}");

            if (user is null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return Ok(new { message = $"User with ID {id} has deleted." });
        }
    }
}
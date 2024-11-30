using Microsoft.AspNetCore.Mvc;
using dotnet_web_api.Controllers;
using dotnet_web_api.Database;
using dotnet_web_api.Services;
using dotnet_web_api.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace dotnet_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly LoggingService _loggingService;

        private readonly ValidatorService _validatorService;

        public AuthController(ApplicationDbContext context, LoggingService loggingService, ValidatorService validatorService)
        {
            _context = context;
            _loggingService = loggingService;
            _validatorService = validatorService;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<User>> SignIn(AuthModel authUser)
        {
            if (_validatorService.IsBodyAuthToNull([authUser.Email ,authUser.Password])) {
                return BadRequest(new { message = "Some request body for authenticate to be incomplete." });
            }

            if (!_validatorService.IsEmailFormat(authUser.Email))
            {
                return BadRequest(new { message = "Email is invalid format." });
            }

            var findUser = await _context.Users.FirstOrDefaultAsync(u => u.user_email == authUser.Email && u.user_password == authUser.Password);

            if (findUser is null)
            {
                return NotFound(new { message = "Email or password is invalid." });
            }

            HttpContext.Session.SetInt32("UserId", findUser.Id);
            HttpContext.Session.SetString("UserName", findUser.user_name);
            
            await _loggingService.AddAuthLog(new AuthLog()
            {
                userRole = findUser.user_role == 1 ? "Admin" : "User",
                action = "Login",
                description = "User has signed in.",
                ip_address = HttpContext.Connection.RemoteIpAddress?.ToString(),
                userId = findUser.Id.ToString(),
            });

            return Ok(new { message = "Sign in success." });
        }

        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUp(User user)
        {
            if (String.IsNullOrEmpty(user.user_name) && String.IsNullOrEmpty(user.user_email) && String.IsNullOrEmpty(user.user_password))
            {
                return BadRequest(new { message = "Name ,Email and Password are required." });
            }

            var findUser = await _context.Users.FirstOrDefaultAsync(u => u.user_email == user.user_email);

            if (findUser is not null)
            {
                return BadRequest(new { message = "Email is already exist." });
            }

            if (!_validatorService.IsEmailFormat(user.user_email))
            {
                return BadRequest(new { message = "Email is invalid format." });
            }

            if (_validatorService.IsPasswordStrong(user.user_password))
            {
                return BadRequest(new { message = "Password is weak." });
            }
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _loggingService.AddAuthLog(new AuthLog()
            {
                userId = user.Id.ToString(),
                action = "Sign Up",
                description = "Registed new user.",
                ip_address = HttpContext.Connection.RemoteIpAddress?.ToString(),
                userRole = "Admin"
            });
            return CreatedAtAction(nameof(SignIn), new { id = user.Id }, user);
        }

        [HttpGet("checkSession")]
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            
            if (userId is null || string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { message = "Session is not found or not found." });
            }
            return Ok(new { message = "Session is active." ,userId ,userName});
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await _loggingService.AddAuthLog(new AuthLog()
            {
                action = "Log out",
                description = $"User id {HttpContext.Session.GetString("UserId")} has logged out.",
                userId = HttpContext.Session.GetInt32("UserId").ToString(),
            });
            HttpContext.Session.Clear();
            return Ok(new { message = "Sign out success." });
        }
    }
}

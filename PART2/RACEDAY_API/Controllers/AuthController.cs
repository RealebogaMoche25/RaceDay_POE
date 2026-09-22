using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

namespace RACEDAY_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AuthController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.FirstName) ||
                string.IsNullOrWhiteSpace(newUser.LastName) ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.PasswordHash) ||
                string.IsNullOrWhiteSpace(newUser.Role))
            {
                return BadRequest("All required fields must be provided.");
            }

            if (newUser.Role != "Organiser" &&
                newUser.Role != "Participant")
            {
                return BadRequest("Role must be Organiser or Participant.");
            }

            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == newUser.Email);

            if (existingUser != null)
            {
                return Conflict("Email already exists.");
            }

            newUser.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            newUser.PasswordHash = "";

            return StatusCode(201, newUser);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login(User loginUser)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == loginUser.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            bool passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    loginUser.PasswordHash,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok(new
            {
                message = "Login successful.",
                userId = user.UserId,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role
            });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                message = "Logged out successfully."
            });
        }
    }
}

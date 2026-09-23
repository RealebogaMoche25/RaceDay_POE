using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

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
        public IActionResult Register(RegisterDto newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.FirstName) ||
                string.IsNullOrWhiteSpace(newUser.LastName) ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.Password) ||
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

            var user = new User
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                Role = newUser.Role,
                PhoneNumber = newUser.PhoneNumber
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return StatusCode(201, new
            {
                message = "Registration successful.",
                userId = user.UserId,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role,
                phoneNumber = user.PhoneNumber
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginUser)
        {
            if (string.IsNullOrWhiteSpace(loginUser.Email) ||
                string.IsNullOrWhiteSpace(loginUser.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Email == loginUser.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(
                loginUser.Password,
                user.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new
            {
                message = "Logged out successfully."
            });
        }

        // GET: api/auth/access-denied
        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return StatusCode(403, "Access denied.");
        }
    }
}
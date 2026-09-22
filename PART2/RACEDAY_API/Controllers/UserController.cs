using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

namespace RACEDAY_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.PhoneNumber,
                user.ProfilePictureUrl
            });
        }

        // PUT: api/users/profile
        [HttpPut("profile")]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Profile updated successfully.",
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.PhoneNumber,
                user.ProfilePictureUrl
            });
        }
    }
}

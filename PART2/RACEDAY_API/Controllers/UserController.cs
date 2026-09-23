using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

namespace RACEDAY_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/users/me
        [HttpGet("me")]
        public IActionResult GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

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

        // PUT: api/users/me
        [HttpPut("me")]
        public IActionResult UpdateProfile(User updatedUser)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

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
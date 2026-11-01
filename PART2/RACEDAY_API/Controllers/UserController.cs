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

        /// <summary>
        /// Retrieves the profile of the authenticated user.
        /// </summary>
        // GET: api/users/me
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateProfile(User updatedUser)
        {
            Console.WriteLine("PROFILE UPDATE REQUEST RECEIVED");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                Console.WriteLine("USER ID CLAIM NOT FOUND");
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                Console.WriteLine("USER NOT FOUND");
                return NotFound("User not found.");
            }

            Console.WriteLine($"Updating user ID: {userId}");
            Console.WriteLine($"New first name: {updatedUser.FirstName}");
            Console.WriteLine($"New last name: {updatedUser.LastName}");
            Console.WriteLine($"New email: {updatedUser.Email}");
            Console.WriteLine($"New phone: {updatedUser.PhoneNumber}");

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;

            _context.SaveChanges();

            Console.WriteLine("PROFILE UPDATE SAVED SUCCESSFULLY");

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

        /// <summary>
        /// Retrieves basic participant details by user ID.
        /// Only organisers can access this endpoint.
        /// </summary>
        // GET: api/users/participants/{id}
        [HttpGet("participants/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetParticipant(int id)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.UserId == id && u.Role == "Participant");

            if (user == null)
            {
                return NotFound("Participant not found.");
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName
            });
        }

    }
}
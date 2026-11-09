using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;
using RACEDAY_API.Services;

namespace RACEDAY_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly AzureBlobStorageService _blobStorageService;

        public UserController(
    ApplicationDBContext context,
    AzureBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
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

            string? profilePictureViewUrl = null;

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            {
                profilePictureViewUrl =
                    _blobStorageService.GetImageReadUrl(
                        user.ProfilePictureUrl,
                        "profile-pictures");
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.PhoneNumber,
                user.ProfilePictureUrl,
                ProfilePictureViewUrl = profilePictureViewUrl
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
        /// Uploads a profile picture for the authenticated user.
        /// </summary>
        // POST: api/users/me/profile-picture
        [HttpPost("me/profile-picture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProfilePicture(
           IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("The image cannot exceed 5 MB.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            try
            {
                var imageUrl = await _blobStorageService.UploadImageAsync(
                    file,
                    "profile-pictures");

                user.ProfilePictureUrl = imageUrl;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profile picture uploaded successfully.",
                    user.UserId,
                    user.ProfilePictureUrl
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Unable to upload profile picture. Please try again.");
            }
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

namespace RACEDAY_API.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all categories associated with a specific event.
        /// </summary>
        // GET: api/events/{eventId}/categories
        // Anyone can view categories
        [HttpGet("api/events/{eventId}/categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEventCategories(int eventId)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var categories = _context.Categories
                .Where(c => c.EventId == eventId)
                .ToList();

            return Ok(categories);
        }

        /// <summary>
        /// Creates a new category for an event. Only the Organiser who owns the event can create a category.
        /// </summary>
        // POST: api/events/{eventId}/categories
        // Only the Organiser who owns the event can create a category
        [Authorize(Roles = "Organiser")]
        [HttpPost("api/events/{eventId}/categories")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateCategory(
            int eventId,
            Category newCategory)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int organiserId = int.Parse(userIdClaim.Value);

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            newCategory.EventId = eventId;

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = newCategory.CategoryId },
                newCategory);
        }

        /// <summary>
        /// Retrieves a specific category by its category ID.
        /// </summary>
        // GET: api/categories/{id}
        // Anyone can view a category
        [HttpGet("api/categories/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        /// <summary>
        /// Updates an existing category. Only the Organiser who owns the associated event can update it.
        /// </summary>
        // PUT: api/categories/{id}
        // Only the Organiser who owns the event can update the category
        [Authorize(Roles = "Organiser")]
        [HttpPut("api/categories/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCategory(
            int id,
            Category updatedCategory)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            var eventItem = _context.Events.Find(category.EventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int organiserId = int.Parse(userIdClaim.Value);

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            category.CategoryName = updatedCategory.CategoryName;
            category.CategoryDescription = updatedCategory.CategoryDescription;

            _context.SaveChanges();

            return Ok(category);
        }

        /// <summary>
        /// Deletes an existing category. Only the Organiser who owns the associated event can delete it.
        /// </summary>
        // DELETE: api/categories/{id}
        // Only the Organiser who owns the event can delete the category
        [Authorize(Roles = "Organiser")]
        [HttpDelete("api/categories/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            var eventItem = _context.Events.Find(category.EventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int organiserId = int.Parse(userIdClaim.Value);

            if (eventItem.OrganiserId != organiserId)
            {
                return Forbid();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
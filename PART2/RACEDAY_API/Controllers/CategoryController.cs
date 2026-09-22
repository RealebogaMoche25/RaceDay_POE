using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

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

        // GET: api/events/{eventId}/categories
        [HttpGet("api/events/{eventId}/categories")]
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

        // POST: api/events/{eventId}/categories
        [HttpPost("api/events/{eventId}/categories")]
        public IActionResult CreateCategory(
            int eventId,
            Category newCategory)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            newCategory.EventId = eventId;

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = newCategory.CategoryId },
                newCategory);
        }

        // GET: api/categories/{id}
        [HttpGet("api/categories/{id}")]
        public IActionResult GetCategory(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/categories/{id}
        [HttpPut("api/categories/{id}")]
        public IActionResult UpdateCategory(
            int id,
            Category updatedCategory)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = updatedCategory.CategoryName;
            category.CategoryDescription = updatedCategory.CategoryDescription;

            _context.SaveChanges();

            return Ok(category);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("api/categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

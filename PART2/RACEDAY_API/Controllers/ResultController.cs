using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

namespace RACEDAY_API.Controllers
{
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ResultController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/events/{eventId}/results
        [HttpPost("api/events/{eventId}/results")]
        public IActionResult CreateResult(
            int eventId,
            Result newResult)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            _context.Results.Add(newResult);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = newResult.ResultId },
                newResult);
        }

        // GET: api/results/my
        [HttpGet("api/results/my")]
        public IActionResult GetMyResults()
        {
            // Authentication/session logic will be added here.
            // For now, return all results for testing.

            return Ok(_context.Results.ToList());
        }

        // GET: api/events/{eventId}/results
        [HttpGet("api/events/{eventId}/results")]
        public IActionResult GetEventResults(int eventId)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var results = _context.Results
                .Where(r => _context.Enrolments
                    .Any(e => e.EnrolmentId == r.EnrolmentId
                          && e.EventId == eventId))
                .ToList();

            return Ok(results);
        }

        // GET: api/results/{id}
        [HttpGet("api/results/{id}")]
        public IActionResult GetResult(int id)
        {
            var result = _context.Results.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
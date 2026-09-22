using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

namespace RACEDAY_API.Controllers
{
    [ApiController]
    public class EnrolmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EnrolmentController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/events/{eventId}/enrolments
        [HttpPost("api/events/{eventId}/enrolments")]
        public IActionResult CreateEnrolment(
            int eventId,
            Enrolment newEnrolment)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            newEnrolment.EventId = eventId;
            newEnrolment.EnrolmentDate = DateTime.Now;
            newEnrolment.EnrolmentStatus = "Active";

            _context.Enrolments.Add(newEnrolment);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = newEnrolment.EnrolmentId },
                newEnrolment);
        }

        // GET: api/enrolments/my
        [HttpGet("api/enrolments/my")]
        public IActionResult GetMyEnrolments()
        {
            // Authentication/session logic will be added here.
            // For now, return all enrolments for testing.

            return Ok(_context.Enrolments.ToList());
        }

        // GET: api/events/{eventId}/enrolments
        [HttpGet("api/events/{eventId}/enrolments")]
        public IActionResult GetEventEnrolments(int eventId)
        {
            var eventItem = _context.Events.Find(eventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            var enrolments = _context.Enrolments
                .Where(e => e.EventId == eventId)
                .ToList();

            return Ok(enrolments);
        }

        // GET: api/enrolments/{id}
        [HttpGet("api/enrolments/{id}")]
        public IActionResult GetEnrolment(int id)
        {
            var enrolment = _context.Enrolments.Find(id);

            if (enrolment == null)
            {
                return NotFound();
            }

            return Ok(enrolment);
        }
    }
}

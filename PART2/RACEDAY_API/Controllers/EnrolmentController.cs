using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

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

        // POST: api/enrolments
        [Authorize(Roles = "Participant")]
        [HttpPost("api/enrolments")]
        public IActionResult CreateEnrolment(Enrolment newEnrolment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int participantId = int.Parse(userIdClaim.Value);

            var eventItem = _context.Events.Find(newEnrolment.EventId);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            newEnrolment.ParticipantId = participantId;
            newEnrolment.EnrolmentDate = DateTime.Now;
            newEnrolment.EnrolmentStatus = "Active";

            _context.Enrolments.Add(newEnrolment);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = newEnrolment.EnrolmentId },
                newEnrolment);
        }

        // GET: api/enrolments/mine
        [Authorize(Roles = "Participant")]
        [HttpGet("api/enrolments/mine")]
        public IActionResult GetMyEnrolments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int participantId = int.Parse(userIdClaim.Value);

            var enrolments = _context.Enrolments
                .Where(e => e.ParticipantId == participantId)
                .ToList();

            return Ok(enrolments);
        }

        // GET: api/events/{eventId}/enrolments
        [Authorize(Roles = "Organiser")]
        [HttpGet("api/events/{eventId}/enrolments")]
        public IActionResult GetEventEnrolments(int eventId)
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

            var enrolments = _context.Enrolments
                .Where(e => e.EventId == eventId)
                .ToList();

            return Ok(enrolments);
        }

        // GET: api/enrolments/{id}
        [Authorize]
        [HttpGet("api/enrolments/{id}")]
        public IActionResult GetEnrolment(int id)
        {
            var enrolment = _context.Enrolments.Find(id);

            if (enrolment == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            if (User.IsInRole("Participant"))
            {
                if (enrolment.ParticipantId != userId)
                {
                    return Forbid();
                }
            }
            else if (User.IsInRole("Organiser"))
            {
                var eventItem = _context.Events.Find(enrolment.EventId);

                if (eventItem == null)
                {
                    return NotFound("Event not found.");
                }

                if (eventItem.OrganiserId != userId)
                {
                    return Forbid();
                }
            }

            return Ok(enrolment);
        }
    }
}
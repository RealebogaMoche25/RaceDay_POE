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

        /// <summary>
        /// Enrols the authenticated Participant in an event.
        /// </summary>
        // POST: api/enrolments
        [Authorize(Roles = "Participant")]
        [HttpPost("api/enrolments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Retrieves all event enrolments belonging to the authenticated Participant.
        /// </summary>
        // GET: api/enrolments/mine
        [Authorize(Roles = "Participant")]
        [HttpGet("api/enrolments/mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Retrieves all enrolments for an event. Only the Organiser who owns the event can view them.
        /// </summary>
        // GET: api/events/{eventId}/enrolments
        [Authorize(Roles = "Organiser")]
        [HttpGet("api/events/{eventId}/enrolments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Retrieves a specific enrolment. Participants can view their own enrolments, while Organisers can view enrolments for their own events.
        /// </summary>
        // GET: api/enrolments/{id}
        [Authorize]
        [HttpGet("api/enrolments/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

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

        /// <summary>
        /// Creates a result for a participant's event enrolment. Only the Organiser who owns the event can create results.
        /// </summary>
        // POST: api/results
        [Authorize(Roles = "Organiser")]
        [HttpPost("api/results")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateResult(Result newResult)
        {
            var enrolment = _context.Enrolments
                .Find(newResult.EnrolmentId);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            var eventItem = _context.Events
                .Find(enrolment.EventId);

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

            _context.Results.Add(newResult);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = newResult.ResultId },
                newResult);
        }

        /// <summary>
        /// Updates an existing result. Only the Organiser who owns the associated event can update it.
        /// </summary>
        // PUT: api/results/{id}
        [Authorize(Roles = "Organiser")]
        [HttpPut("api/results/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateResult(
            int id,
            Result updatedResult)
        {
            var result = _context.Results.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            var enrolment = _context.Enrolments
                .Find(result.EnrolmentId);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
            }

            var eventItem = _context.Events
                .Find(enrolment.EventId);

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

            result.FinishTime = updatedResult.FinishTime;
            result.FinishingPosition = updatedResult.FinishingPosition;

            _context.SaveChanges();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all results belonging to the authenticated Participant.
        /// </summary>
        // GET: api/results/mine
        [Authorize(Roles = "Participant")]
        [HttpGet("api/results/mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetMyResults()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int participantId = int.Parse(userIdClaim.Value);

            var results = _context.Results
                .Where(r => _context.Enrolments
                    .Any(e => e.EnrolmentId == r.EnrolmentId
                          && e.ParticipantId == participantId))
                .ToList();

            return Ok(results);
        }

        /// <summary>
        /// Retrieves all results for an event. Only the Organiser who owns the event can view its results.
        /// </summary>
        // GET: api/events/{eventId}/results
        [Authorize(Roles = "Organiser")]
        [HttpGet("api/events/{eventId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEventResults(int eventId)
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

            var results = _context.Results
                .Where(r => _context.Enrolments
                    .Any(e => e.EnrolmentId == r.EnrolmentId
                          && e.EventId == eventId))
                .ToList();

            return Ok(results);
        }

        /// <summary>
        /// Retrieves a specific result. Participants can view their own results, while Organisers can view results for their own events.
        /// </summary>
        // GET: api/results/{id}
        [Authorize]
        [HttpGet("api/results/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetResult(int id)
        {
            var result = _context.Results.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            var enrolment = _context.Enrolments
                .Find(result.EnrolmentId);

            if (enrolment == null)
            {
                return NotFound("Enrolment not found.");
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

            return Ok(result);
        }
    }
}
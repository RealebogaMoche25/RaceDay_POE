using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;

namespace RACEDAY_API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EventController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all events available on the RaceDay platform.
        /// </summary>
        // GET: api/events
        // Anyone can view events
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }

        /// <summary>
        /// Retrieves a specific event by its event ID.
        /// </summary>
        // GET: api/events/{id}
        // Anyone can view a specific event
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEvent(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        /// <summary>
        /// Creates a new RaceDay event. Only authenticated Organisers can create events.
        /// </summary>
        // POST: api/events
        // Only Organisers can create events
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CreateEvent(Event newEvent)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int organiserId = int.Parse(userIdClaim.Value);

            newEvent.OrganiserId = organiserId;

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.EventId },
                newEvent);
        }

        /// <summary>
        /// Updates an existing event. Only the Organiser who owns the event can update it.
        /// </summary>
        // PUT: api/events/{id}
        // Only the Organiser who owns the event can update it
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateEvent(int id, Event updatedEvent)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
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

            eventItem.EventName = updatedEvent.EventName;
            eventItem.EventDescription = updatedEvent.EventDescription;
            eventItem.EventDate = updatedEvent.EventDate;
            eventItem.EventLocation = updatedEvent.EventLocation;
            eventItem.EventDistance = updatedEvent.EventDistance;
            eventItem.EventType = updatedEvent.EventType;
            eventItem.BannerImageUrl = updatedEvent.BannerImageUrl;

            _context.SaveChanges();

            return Ok(eventItem);
        }

        /// <summary>
        /// Deletes an existing event. Only the Organiser who owns the event can delete it.
        /// </summary>
        // DELETE: api/events/{id}
        // Only the Organiser who owns the event can delete the event
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteEvent(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
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

            _context.Events.Remove(eventItem);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
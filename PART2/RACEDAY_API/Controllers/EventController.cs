using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;

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

        // GET: api/events
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // POST: api/events
        [HttpPost]
        public IActionResult CreateEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.EventId },
                newEvent);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, Event updatedEvent)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            eventItem.EventName = updatedEvent.EventName;
            eventItem.EventDescription = updatedEvent.EventDescription;
            eventItem.EventDate = updatedEvent.EventDate;
            eventItem.EventLocation = updatedEvent.EventLocation;
            eventItem.EventDistance = updatedEvent.EventDistance;
            eventItem.EventType = updatedEvent.EventType;
            eventItem.BannerImageUrl = updatedEvent.BannerImageUrl;
            eventItem.OrganiserId = updatedEvent.OrganiserId;

            _context.SaveChanges();

            return Ok(eventItem);
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var eventItem = _context.Events.Find(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
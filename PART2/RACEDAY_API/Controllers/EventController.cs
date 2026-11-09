using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using RACEDAY_API.Models;
using System.Security.Claims;
using RACEDAY_API.Services;

namespace RACEDAY_API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly AzureBlobStorageService _blobStorageService;

        public EventController(
    ApplicationDBContext context,
    AzureBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
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
            var events = _context.Events.ToList();

            var eventResponses = events.Select(eventItem =>
            {
                string? bannerImageViewUrl = null;

                if (!string.IsNullOrWhiteSpace(eventItem.BannerImageUrl))
                {
                    bannerImageViewUrl =
                        _blobStorageService.GetImageReadUrl(
                            eventItem.BannerImageUrl,
                            "event-banners");
                }

                return new
                {
                    eventItem.EventId,
                    eventItem.EventName,
                    eventItem.EventDescription,
                    eventItem.EventDate,
                    eventItem.EventLocation,
                    eventItem.EventDistance,
                    eventItem.EventType,
                    eventItem.BannerImageUrl,
                    eventItem.OrganiserId,
                    BannerImageViewUrl = bannerImageViewUrl
                };
            }).ToList();

            return Ok(eventResponses);
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

            string? bannerImageViewUrl = null;

            if (!string.IsNullOrWhiteSpace(eventItem.BannerImageUrl))
            {
                bannerImageViewUrl =
                    _blobStorageService.GetImageReadUrl(
                        eventItem.BannerImageUrl,
                        "event-banners");
            }

            return Ok(new
            {
                eventItem.EventId,
                eventItem.EventName,
                eventItem.EventDescription,
                eventItem.EventDate,
                eventItem.EventLocation,
                eventItem.EventDistance,
                eventItem.EventType,
                eventItem.BannerImageUrl,
                eventItem.OrganiserId,
                BannerImageViewUrl = bannerImageViewUrl
            });
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
        /// Uploads an event banner image to Azure Blob Storage.
        /// Only the Organiser who owns the event can upload its banner.
        /// </summary>
        // POST: api/events/{id}/banner
        [Authorize(Roles = "Organiser")]
        [HttpPost("{id}/banner")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadEventBanner(
            int id,
            IFormFile file)
        {
            var eventItem = _context.Events.Find(id);

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

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("The image cannot exceed 5 MB.");
            }

            try
            {
                var imageUrl = await _blobStorageService.UploadImageAsync(
                    file,
                    "event-banners");

                eventItem.BannerImageUrl = imageUrl;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Event banner uploaded successfully.",
                    eventItem.EventId,
                    eventItem.BannerImageUrl
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
                    "Unable to upload event banner. Please try again.");
            }
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
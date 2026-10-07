using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;
using System.Security.Claims;

namespace RACEDAY_API.Controllers
{
    [Route("api/routes")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public RouteController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all routes available on the RaceDay platform.
        /// </summary>
        // GET: api/routes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetRoutes()
        {
            return Ok(_context.Routes.ToList());
        }

        /// <summary>
        /// Retrieves a specific route by its route ID.
        /// </summary>
        // GET: api/routes/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRoute(int id)
        {
            var route = _context.Routes.Find(id);

            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        /// <summary>
        /// Creates a new route for an event. Only the Organiser who owns the event can create a route.
        /// </summary>
        // POST: api/routes
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateRoute(
            RACEDAY_API.Models.Route newRoute)
        {
            var eventItem = _context.Events.Find(newRoute.EventId);

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

            _context.Routes.Add(newRoute);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetRoute),
                new { id = newRoute.RouteId },
                newRoute);
        }

        /// <summary>
        /// Updates an existing route. Only the Organiser who owns the associated event can update it.
        /// </summary>
        // PUT: api/routes/{id}
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateRoute(
            int id,
            RACEDAY_API.Models.Route updatedRoute)
        {
            var route = _context.Routes.Find(id);

            if (route == null)
            {
                return NotFound();
            }

            var eventItem = _context.Events.Find(route.EventId);

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

            route.RouteName = updatedRoute.RouteName;
            route.RouteDescription = updatedRoute.RouteDescription;
            route.RouteUrl = updatedRoute.RouteUrl;
            route.RouteLocation = updatedRoute.RouteLocation;

            _context.SaveChanges();

            return Ok(route);
        }

        /// <summary>
        /// Deletes an existing route. Only the Organiser who owns the associated event can delete it.
        /// </summary>
        // DELETE: api/routes/{id}
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRoute(int id)
        {
            var route = _context.Routes.Find(id);

            if (route == null)
            {
                return NotFound();
            }

            var eventItem = _context.Events.Find(route.EventId);

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

            _context.Routes.Remove(route);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
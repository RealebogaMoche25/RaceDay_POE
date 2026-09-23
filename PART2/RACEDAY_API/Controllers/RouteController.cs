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

        // GET: api/routes
        [HttpGet]
        public IActionResult GetRoutes()
        {
            return Ok(_context.Routes.ToList());
        }

        // GET: api/routes/{id}
        [HttpGet("{id}")]
        public IActionResult GetRoute(int id)
        {
            var route = _context.Routes.Find(id);

            if (route == null)
            {
                return NotFound();
            }

            return Ok(route);
        }

        // POST: api/routes
        [Authorize(Roles = "Organiser")]
        [HttpPost]
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

        // PUT: api/routes/{id}
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
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

        // DELETE: api/routes/{id}
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
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
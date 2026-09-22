using Microsoft.AspNetCore.Mvc;
using RACEDAY_API.Data;

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
        [HttpPost]
        public IActionResult CreateRoute(
            RACEDAY_API.Models.Route newRoute)
        {
            _context.Routes.Add(newRoute);
            _context.SaveChanges();

            return CreatedAtAction(
                nameof(GetRoute),
                new { id = newRoute.RouteId },
                newRoute);
        }

        // PUT: api/routes/{id}
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

            route.RouteName = updatedRoute.RouteName;
            route.RouteDescription = updatedRoute.RouteDescription;
            route.RouteUrl = updatedRoute.RouteUrl;
            route.RouteLocation = updatedRoute.RouteLocation;
            route.EventId = updatedRoute.EventId;

            _context.SaveChanges();

            return Ok(route);
        }

        // DELETE: api/routes/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteRoute(int id)
        {
            var route = _context.Routes.Find(id);

            if (route == null)
            {
                return NotFound();
            }

            _context.Routes.Remove(route);
            _context.SaveChanges();

            return NoContent();
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    [SessionAuthorize("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.CreateEventAsync(model);

                TempData["SuccessMessage"] =
                    "Your event was created successfully.";

                return RedirectToAction("ManageEvents");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(id);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (raceEvent.OrganiserId != organiserId)
                {
                    return Forbid();
                }

                var model = new CreateEventViewModel
                {
                    EventName = raceEvent.EventName,
                    EventDescription = raceEvent.EventDescription,
                    EventDate = raceEvent.EventDate,
                    EventLocation = raceEvent.EventLocation,
                    EventDistance = raceEvent.EventDistance,
                    EventType = raceEvent.EventType,
                    BannerImageUrl = raceEvent.BannerImageUrl
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load the event for editing.";
                return RedirectToAction("ManageEvents");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id,
            CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiService.UpdateEventAsync(id, model);

                TempData["SuccessMessage"] = "Event updated successfully.";

                return RedirectToAction("ManageEvents");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _apiService.DeleteEventAsync(id);

                TempData["SuccessMessage"] = "Event deleted successfully.";

                return RedirectToAction("ManageEvents");
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("ManageEvents");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ManageEvents()
        {
            try
            {
                var events = await _apiService.GetEventsAsync();

                var organiserId = HttpContext.Session.GetInt32("UserId");

                var organiserEvents = events
                    .Where(e => e.OrganiserId == organiserId)
                    .ToList();

                return View(organiserEvents);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to load your events. Please try again later.";

                return View(new List<EventViewModel>());
            }
        }
    }
}

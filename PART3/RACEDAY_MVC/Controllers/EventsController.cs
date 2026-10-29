
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    [SessionAuthorize]
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _apiService.GetEventsAsync();

                return View(events);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "We couldn't load events. Please try again later.";

                return View(new List<RaceDay.Web.ViewModels.EventViewModel>());
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(id);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                return View(raceEvent);
            }
            catch (HttpRequestException)
            {
                return View("Error");
            }
        }


    }
}


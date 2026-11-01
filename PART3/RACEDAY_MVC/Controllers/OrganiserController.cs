
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
        public async Task<IActionResult> ManageCategories(int eventId)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(eventId);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (raceEvent.OrganiserId != organiserId)
                {
                    return Forbid();
                }

                var categories = await _apiService.GetEventCategoriesAsync(eventId);

                var model = new ManageCategoriesViewModel
                {
                    EventId = raceEvent.EventId,
                    EventName = raceEvent.EventName,
                    Categories = categories
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load event categories.";
                return RedirectToAction("ManageEvents");
            }
        }


        [HttpGet]
        public async Task<IActionResult> CreateCategory(int eventId)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(eventId);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (raceEvent.OrganiserId != organiserId)
                {
                    return Forbid();
                }

                ViewBag.EventId = raceEvent.EventId;
                ViewBag.EventName = raceEvent.EventName;

                return View();
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load the event.";
                return RedirectToAction("ManageEvents");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(int eventId, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EventId = eventId;
                return View(model);
            }

            try
            {
                await _apiService.CreateCategoryAsync(eventId, model);

                TempData["SuccessMessage"] = "Category created successfully.";

                return RedirectToAction("ManageCategories", new { eventId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.EventId = eventId;

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditCategory(int eventId, int categoryId)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(eventId);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (raceEvent.OrganiserId != organiserId)
                {
                    return Forbid();
                }

                var categories = await _apiService.GetEventCategoriesAsync(eventId);

                var category = categories.FirstOrDefault(
                    c => c.CategoryId == categoryId);

                if (category == null)
                {
                    return NotFound();
                }

                ViewBag.EventName = raceEvent.EventName;

                return View(category);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load the category.";
                return RedirectToAction("ManageCategories", new { eventId });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int eventId, int categoryId)
        {
            try
            {
                await _apiService.DeleteCategoryAsync(categoryId);

                TempData["SuccessMessage"] = "Category deleted successfully.";

                return RedirectToAction("ManageCategories", new { eventId });
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction("ManageCategories", new { eventId });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(
            int eventId, int categoryId, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.EventId = eventId;
                model.CategoryId = categoryId;

                await _apiService.UpdateCategoryAsync(categoryId, model);

                TempData["SuccessMessage"] = "Category updated successfully.";

                return RedirectToAction("ManageCategories", new { eventId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
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

        [HttpGet]
        public async Task<IActionResult> ManageEnrolments(int eventId)
        {
            try
            {
                var raceEvent = await _apiService.GetEventByIdAsync(eventId);

                if (raceEvent == null)
                {
                    return NotFound();
                }

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (raceEvent.OrganiserId != organiserId)
                {
                    return Forbid();
                }

                var enrolments = await _apiService.GetEventEnrolmentsAsync(eventId);
                var categories = await _apiService.GetEventCategoriesAsync(eventId);

                var organiserEnrolments = new List<OrganiserEnrolmentViewModel>();

                foreach (var enrolment in enrolments)
                {
                    var participant = await _apiService.GetParticipantAsync(enrolment.ParticipantId);

                    var category = categories.FirstOrDefault(
                        c => c.CategoryId == enrolment.CategoryId);

                    organiserEnrolments.Add(new OrganiserEnrolmentViewModel
                    {
                        EnrolmentId = enrolment.EnrolmentId,
                        ParticipantId = enrolment.ParticipantId,
                        ParticipantName = participant == null
                            ? "Participant unavailable"
                            : $"{participant.FirstName} {participant.LastName}",
                        EventId = enrolment.EventId,
                        CategoryId = enrolment.CategoryId,
                        CategoryName = category?.CategoryName ?? "Category unavailable",
                        EnrolmentDate = enrolment.EnrolmentDate,
                        EnrolmentStatus = enrolment.EnrolmentStatus
                    });
                }

                ViewBag.EventName = raceEvent.EventName;
                ViewBag.EventId = eventId;

                return View(organiserEnrolments);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to load event enrolments. Please try again.";
                return RedirectToAction("ManageEvents");
            }
        }

    }
}

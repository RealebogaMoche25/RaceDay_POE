using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    [SessionAuthorize("Participant")]
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> MyEvents()
        {
            try
            {
                var enrolments = await _apiService.GetMyEnrolmentsAsync();

                var myEvents = new List<MyEventViewModel>();

                foreach (var enrolment in enrolments)
                {
                    var raceEvent = await _apiService.GetEventByIdAsync(enrolment.EventId);

                    if (raceEvent == null)
                    {
                        continue;
                    }

                    var categories = await _apiService.GetEventCategoriesAsync(enrolment.EventId);

                    var category = categories.FirstOrDefault(
                        c => c.CategoryId == enrolment.CategoryId);

                    myEvents.Add(new MyEventViewModel
                    {
                        EnrolmentId = enrolment.EnrolmentId,
                        EventId = enrolment.EventId,
                        EventName = raceEvent.EventName,
                        EventDate = raceEvent.EventDate,
                        EventLocation = raceEvent.EventLocation,
                        CategoryName = category?.CategoryName ?? "Category unavailable",
                        EnrolmentStatus = enrolment.EnrolmentStatus,
                        EnrolmentDate = enrolment.EnrolmentDate
                    });
                }

                return View("~/Views/Participant/MyEvents.cshtml", myEvents);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "We couldn't load your events. Please try again later.";

                return View("~/Views/Participant/MyEvents.cshtml",
    new List<MyEventViewModel>());
            }
        }
    }
}

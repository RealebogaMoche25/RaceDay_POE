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

        [HttpGet]
        public async Task<IActionResult> MyResults()
        {
            try
            {
                var results = await _apiService.GetMyResultsAsync();
                var enrolments = await _apiService.GetMyEnrolmentsAsync();

                var myResults = new List<MyResultViewModel>();

                foreach (var result in results)
                {
                    var enrolment = enrolments.FirstOrDefault(
                        e => e.EnrolmentId == result.EnrolmentId);

                    if (enrolment == null)
                    {
                        continue;
                    }

                    var raceEvent = await _apiService.GetEventByIdAsync(
                        enrolment.EventId);

                    if (raceEvent == null)
                    {
                        continue;
                    }

                    // Retrieve the event categories.
                    var categories = await _apiService.GetEventCategoriesAsync(
                        raceEvent.EventId);

                    // Find the participant's selected category.
                    var category = categories.FirstOrDefault(
                        c => c.CategoryId == enrolment.CategoryId);

                    var totalFinishers = await _apiService.GetEventFinisherCountAsync(
    raceEvent.EventId);

                    myResults.Add(new MyResultViewModel
                    {
                        ResultId = result.ResultId,
                        EventId = raceEvent.EventId,
                        EventName = raceEvent.EventName,
                        EventDate = raceEvent.EventDate,
                        EventLocation = raceEvent.EventLocation,
                        CategoryName = category?.CategoryName ?? "Not specified",
                        FinishTime = result.FinishTime,
                        FinishingPosition = result.FinishingPosition,
                        TotalFinishers = totalFinishers
                    });
                }

                return View(myResults);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "We couldn't load your results. Please try again later.";

                return View(new List<MyResultViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var profile = await _apiService.GetMyProfileAsync();

                if (profile == null)
                {
                    TempData["ErrorMessage"] = "Unable to retrieve your profile.";
                    return RedirectToAction("MyEvents");
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "We couldn't load your profile. Please try again later.";

                return RedirectToAction("MyEvents");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updatedProfile = await _apiService.UpdateMyProfileAsync(model);

                if (updatedProfile == null)
                {
                    ModelState.AddModelError(
                        string.Empty, "Unable to update your profile.");

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Your profile has been updated successfully.";

                return RedirectToAction("MyProfile");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }


    }
}

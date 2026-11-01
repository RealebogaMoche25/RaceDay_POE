
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Filters;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    [SessionAuthorize("Participant")]
    public class EnrolmentController : Controller
    {
        private readonly ApiService _apiService;

        public EnrolmentController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, int categoryId)
        {
            if (eventId <= 0 || categoryId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Please select a valid event category.";

                return RedirectToAction(
                    "Details", "Events", new { id = eventId });
            }

            try
            {
                await _apiService.CreateEnrolmentAsync(
                    eventId, categoryId);

                TempData["SuccessMessage"] =
                    "You have successfully enrolled in this event!";

                return RedirectToAction(
                    "MyEvents", "Participant");
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized =>
                        "Your session has expired. Please log in again.",

                    System.Net.HttpStatusCode.Forbidden =>
                        "You are not authorised to enrol in this event.",

                    System.Net.HttpStatusCode.Conflict =>
                        "You may already be enrolled in this event.",

                    _ => "Unable to complete your enrolment. Please try again."
                };

                return RedirectToAction(
                    "Details", "Events", new { id = eventId });
            }
        }
    }
}

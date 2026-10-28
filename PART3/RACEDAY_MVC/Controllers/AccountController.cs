
using Microsoft.AspNetCore.Mvc;
using RACEDAY_MVC.ViewModels;
using RACEDAY_MVC.Services;

namespace RACEDAY_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AccountController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authApiService.RegisterAsync(model);

                TempData["SuccessMessage"] =
                    result.Message ?? "Registration successful.";

                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to complete registration. Please try again.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Content(
                "Registration succeeded. The MVC login page will be implemented in the next step.");
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.ViewModels;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
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
            return View(new LoginViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authApiService.LoginAsync(model);


                if (result.UserId <= 0 ||
                    string.IsNullOrWhiteSpace(result.Role))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The API returned incomplete user information.");

                    return View(model);
                }

                HttpContext.Session.SetInt32("UserId", result.UserId);

                HttpContext.Session.SetString(
                    "FirstName",
                    result.FirstName ?? string.Empty);

                HttpContext.Session.SetString(
                    "LastName",
                    result.LastName ?? string.Empty);

                HttpContext.Session.SetString(
                    "Email",
                    result.Email ?? string.Empty);

                HttpContext.Session.SetString(
                    "Role",
                    result.Role);

                TempData["SuccessMessage"] =
                    $"Welcome back, {result.FirstName}!";

                return RedirectToAction("Index", "Home");

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
                    "Unable to log in. Please try again.");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have been logged out successfully.";

            return RedirectToAction("Index", "Home");
        }


    }
}

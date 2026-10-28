
using System.Net.Http.Json;
using RACEDAY_MVC.ViewModels;

namespace RACEDAY_MVC.Services
{
    public class AuthApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthResponseViewModel> RegisterAsync(
            RegisterViewModel model)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var response = await client.PostAsJsonAsync(
                "api/auth/register",
                new
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    email = model.Email,
                    password = model.Password,
                    role = model.Role,
                    phoneNumber = model.PhoneNumber
                });

            var result = await response.Content
                .ReadFromJsonAsync<AuthResponseViewModel>();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    result?.Message ?? "Registration failed.",
                    null,
                    response.StatusCode);
            }

            return result ?? new AuthResponseViewModel
            {
                Message = "Registration completed."
            };
        }
    }
}


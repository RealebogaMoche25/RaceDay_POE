
using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.Web.ViewModels;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateApiClient()
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

            var apiCookie = _httpContextAccessor.HttpContext?
                .Session.GetString("ApiAuthCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                var cookiePair = apiCookie.Split(';')[0];

                client.DefaultRequestHeaders.Remove("Cookie");
                client.DefaultRequestHeaders.TryAddWithoutValidation(
                    "Cookie", cookiePair);
            }

            return client;
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/events");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Unable to retrieve events. API returned status code {(int)response.StatusCode}.");
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            return events ?? new List<EventViewModel>();
        }

        public async Task<EventViewModel> CreateEventAsync(
    CreateEventViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync(
                "api/events",
                model);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"API Error: {(int)response.StatusCode} - {errorDetails}",
                    null,
                    response.StatusCode);
            }

            var createdEvent =
                await response.Content.ReadFromJsonAsync<EventViewModel>();

            return createdEvent ??
                throw new HttpRequestException(
                    "The API returned an empty event response.");
        }



        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/events/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Unable to retrieve event. API returned status code {(int)response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<EventViewModel>();
        }


        public async Task<List<CategoryViewModel>> GetEventCategoriesAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync(
                $"api/events/{eventId}/categories");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<CategoryViewModel>();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Unable to retrieve event categories. API returned status code {(int)response.StatusCode}.");
            }

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            return categories ?? new List<CategoryViewModel>();
        }


        public async Task<EnrolmentViewModel> CreateEnrolmentAsync(
            int eventId,
            int categoryId)
        {
            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync(
                "api/enrolments",
                new
                {
                    EventId = eventId,
                    CategoryId = categoryId
                });

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Unable to enrol in the event. API returned status code {(int)response.StatusCode}.");
            }

            var enrolment = await response.Content
                .ReadFromJsonAsync<EnrolmentViewModel>();

            return enrolment ?? throw new HttpRequestException(
                "The API returned an empty enrolment response.");
        }


        public async Task<EventViewModel> UpdateEventAsync(
            int eventId,
            CreateEventViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync(
                $"api/events/{eventId}",
                model);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to update event. API returned {(int)response.StatusCode}: {errorDetails}",
                    null,
                    response.StatusCode);
            }

            var updatedEvent =
                await response.Content.ReadFromJsonAsync<EventViewModel>();

            return updatedEvent ??
                throw new HttpRequestException(
                    "The API returned an empty event response.");
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync(
                $"api/events/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to delete event. API returned {(int)response.StatusCode}: {errorDetails}",
                    null,
                    response.StatusCode);
            }
        }

        public async Task<List<EnrolmentViewModel>> GetMyEnrolmentsAsync()
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/enrolments/mine");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Unable to retrieve your enrolments. API returned status code {(int)response.StatusCode}.");
            }

            var enrolments = await response.Content
                .ReadFromJsonAsync<List<EnrolmentViewModel>>();

            return enrolments ?? new List<EnrolmentViewModel>();
        }


    }
}

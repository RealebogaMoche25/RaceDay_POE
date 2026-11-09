
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



        // Create a category for an event
        public async Task<CategoryViewModel?> CreateCategoryAsync(
            int eventId, CategoryViewModel model)
        {
            var client = CreateApiClient();

            model.EventId = eventId;

            var response = await client.PostAsJsonAsync(
                $"api/events/{eventId}/categories", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to create category. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<CategoryViewModel>();
        }


        // Update an existing category
        public async Task<CategoryViewModel?> UpdateCategoryAsync(
            int categoryId, CategoryViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync(
                $"api/categories/{categoryId}", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to update category. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<CategoryViewModel>();
        }


        // Delete a category
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync(
                $"api/categories/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to delete category. Status: {response.StatusCode}. {error}");
            }
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

        public async Task<ParticipantViewModel?> GetParticipantAsync(int participantId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/users/participants/{participantId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve participant details. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<ParticipantViewModel>();
        }

        public async Task<List<EnrolmentViewModel>> GetEventEnrolmentsAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/events/{eventId}/enrolments");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve event enrolments. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<EnrolmentViewModel>>()
                   ?? new List<EnrolmentViewModel>();
        }


        public async Task<List<ResultViewModel>> GetEventResultsAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync($"api/events/{eventId}/results");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve event results. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<ResultViewModel>>()
                   ?? new List<ResultViewModel>();
        }

        public async Task<ResultViewModel?> CreateResultAsync(CreateResultViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PostAsJsonAsync("api/results", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to create result. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<ResultViewModel>();
        }

        public async Task<ResultViewModel?> UpdateResultAsync(int resultId, CreateResultViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync($"api/results/{resultId}", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to update result. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<ResultViewModel>();
        }

        public async Task<List<RouteViewModel>> GetEventRoutesAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/routes");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Unable to retrieve routes. Status: {response.StatusCode}. {error}");
            }

            var routes = await response.Content.ReadFromJsonAsync<List<RouteViewModel>>()
                         ?? new List<RouteViewModel>();

            return routes.Where(r => r.EventId == eventId).ToList();
        }

        public async Task<RouteViewModel?> CreateRouteAsync(
            int eventId, CreateRouteViewModel model)
        {
            var client = CreateApiClient();

            var routeData = new
            {
                model.RouteName,
                model.RouteDescription,
                model.RouteUrl,
                model.RouteLocation,
                EventId = eventId
            };

            var response = await client.PostAsJsonAsync("api/routes", routeData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Unable to create route. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<RouteViewModel>();
        }

        public async Task<RouteViewModel?> UpdateRouteAsync(
            int routeId, CreateRouteViewModel model)
        {
            var client = CreateApiClient();

            var routeData = new
            {
                model.RouteName,
                model.RouteDescription,
                model.RouteUrl,
                model.RouteLocation
            };

            var response = await client.PutAsJsonAsync(
                $"api/routes/{routeId}", routeData);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Unable to update route. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<RouteViewModel>();
        }

        public async Task DeleteRouteAsync(int routeId)
        {
            var client = CreateApiClient();

            var response = await client.DeleteAsync($"api/routes/{routeId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Unable to delete route. Status: {response.StatusCode}. {error}");
            }
        }


        public async Task<List<ResultViewModel>> GetMyResultsAsync()
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/results/mine");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve your results. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<List<ResultViewModel>>()
                   ?? new List<ResultViewModel>();
        }


        public async Task<int> GetEventFinisherCountAsync(int eventId)
        {
            var client = CreateApiClient();

            var response = await client.GetAsync(
                $"api/events/{eventId}/results/count");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve event finisher count. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<int>();
        }


        // Retrieve the logged-in user's profile
        public async Task<ProfileViewModel?> GetMyProfileAsync()
        {
            var client = CreateApiClient();

            var response = await client.GetAsync("api/users/me");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to retrieve your profile. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<ProfileViewModel>();
        }


        // Update the logged-in user's profile
        public async Task<ProfileViewModel?> UpdateMyProfileAsync(
            ProfileViewModel model)
        {
            var client = CreateApiClient();

            var response = await client.PutAsJsonAsync(
                "api/users/me", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"PROFILE UPDATE API ERROR: {response.StatusCode} - {error}");

                throw new HttpRequestException(
                    $"Unable to update your profile. Status: {response.StatusCode}. {error}");
            }

            return await response.Content.ReadFromJsonAsync<ProfileViewModel>();
        }


        // Upload the logged-in participant's profile picture
        public async Task<bool> UploadProfilePictureAsync(IFormFile file)
        {
            var client = CreateApiClient();

            using var content = new MultipartFormDataContent();

            using var fileStream = file.OpenReadStream();

            using var fileContent = new StreamContent(fileStream);

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync(
                "api/users/me/profile-picture",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to upload profile picture. Status: {response.StatusCode}. {error}");
            }

            return true;
        }


        // Upload an event banner to Azure Blob Storage
        public async Task<bool> UploadEventBannerAsync(int eventId, IFormFile file)
        {
            var client = CreateApiClient();

            using var content = new MultipartFormDataContent();

            using var fileStream = file.OpenReadStream();

            using var fileContent = new StreamContent(fileStream);

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync(
                $"api/events/{eventId}/banner",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Unable to upload event banner. Status: {response.StatusCode}. {error}");
            }

            return true;
        }


    }
}

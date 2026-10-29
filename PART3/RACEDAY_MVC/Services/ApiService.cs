
using System.Net.Http.Json;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

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


        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("RaceDayApi");

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


    }
}


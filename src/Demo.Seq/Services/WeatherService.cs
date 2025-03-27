using Demo.Seq.Models;
using System.Text.Json;

namespace Demo.Seq.Services
{
    public interface IWeatherService
    {
        Task<Weather[]> GetWeathersAsync();
    }
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _client;

        public WeatherService(HttpClient client)
        {
            _client = client;
        }

        public async Task<Weather[]> GetWeathersAsync()
        {
            var response = await _client.GetAsync("WeatherForecast");

            var st = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Weather[]>(st, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}

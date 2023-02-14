namespace Weather.Specs.Mocks;

public class WeatherApi
{
    private readonly HttpClient _httpClient;

    public WeatherApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetWeatherByCityName(string cityName)
    {
        var result = await _httpClient.GetAsync($"api/city/{cityName}");
        return string.Empty;
    }
}
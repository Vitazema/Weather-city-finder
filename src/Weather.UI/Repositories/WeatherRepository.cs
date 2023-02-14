using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather.Domain;

namespace Weather.UI.Repositories
{
  public class WeatherRepository : IWeatherRepository
  {
    private readonly HttpClient _httpClient;

    public WeatherRepository(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    /// <summary>
    /// Сбор и обработка списка температур с локального WEB API
    /// </summary>
    /// <param name="cityName">Название города</param>
    /// <returns>Список температур для выбранного города</returns>
    public async Task<List<Temperature>> GetWeatherAsync(string cityName)
    {
      try
      {
        var result = await _httpClient.GetAsync(
        $"/api/weather/city/{cityName}");
        var rawResponse = await result.Content.ReadAsStringAsync();

        // Десерилиазовать строковые данные формата JSON
        var temperatures = JsonConvert.DeserializeObject<List<Temperature>>(rawResponse);
        return temperatures;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return null;
      }
    }
  }
}

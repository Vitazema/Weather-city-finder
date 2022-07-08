using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Weather.Domain;
using Weather.Domain.Utils;

namespace Weather.Data
{
  public class WeatherDbContext: DbContext
  {
    public DbSet<City> Cities { get; set; }
    public DbSet<Temperature> Temperatures { get; set; }

    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

    // Код доступа для API Openweatherapi
    private const string APP_ID = "e1acaf295ad1a732578589e9e47709ce";

    public async Task<int> AddTemperatures(City city, params Temperature[] temperatures)
    {
      var existingTemperatures = Temperatures.ToList();

      var newTempCount = 0;

      city.Temperature ??= new List<Temperature>();
      foreach (var temp in temperatures)
      {
        if (!existingTemperatures.Any(exTemp =>
              exTemp.Date == temp.Date &&
              Math.Abs(exTemp.Air - temp.Air) < 0.01))
        {
          city.Temperature.Add(temp);
          newTempCount++;
        }
        else
          Debug.WriteLine($"Temperature with data: D:{temp.Date} and T:{temp.Air} from {temp.City?.Name} Already in database. Will not be added");
      }
      await SaveChangesAsync();
      return newTempCount;
    }

    public async Task<List<Temperature>> GetWeatherByCityNameAsync(string cityName)
    {
      // Возвращаем соединённые данные температуры по заданному городу
      return await Temperatures
        .Join(Cities,
          temp => temp.CityId,
          city => city.Id,
          (temp, city) => new Temperature
          {
            City = city,
            Air = temp.Air,
            CityId = city.Id,
            Date = temp.Date,
            Id = temp.Id
          })
        .Where(city => city.City.Name == cityName)
        .ToListAsync();
    }

    public List<City> GetCities(bool unique = true)
    {
      var cities = Cities.ToList();

      // Забрать уникальный список городов из БД 
      if (unique) return cities
        .GroupBy(c => c.Name)
        .Select(c => c.First())
        .ToList();

      // Забрать весь список (для проверки БД на наличие дубликатов)
      return cities;
    }
    public async Task<List<Temperature>> GetTemperatureFromOpenWeatherApiByCity(City city)
    {
      var temperatures = new List<Temperature>();

      try
      {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.openweathermap.org");
        var response = await client.GetAsync($"/data/2.5/forecast?q={city.Name}&appid={APP_ID}&units=metric");
        response.EnsureSuccessStatusCode();

        // Синхронный вариант
        //var result = await response.Content.ReadAsStringAsync();
        //var rawWeather = JsonSerializer.Deserialize<RootObject>(result);

        // Асинхронное чтение потока данных от API
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var rawWeather = await JsonSerializer.DeserializeAsync<RootObject>(responseStream);

        if (rawWeather == null) return temperatures;
        foreach (var list in rawWeather.list)
        {
          var temp = new Temperature()
          {
            City = city,
            Date = DateTimeUtils.UnixTimeStampToDateTime(list.dt),
            Air = (float)list.main.temp
          };

          temperatures.Add(temp);
        }

        return temperatures;
      }

      catch (HttpRequestException httpRequestException)
      {
        Debug.WriteLine($"Ошибка приёма данных: {httpRequestException.Message}");
        return temperatures;
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Weather.Data;
using Weather.Domain;
using Weather.Domain.Utils;

namespace Weather.Api.Services
{
  /// <summary>
  /// Фоновый сервис для периодической сборки новых температурных данных по городам от OpenWeatherMap
  /// </summary>
  public class WeatherService : IHostedService, IDisposable
  {
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scope;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private Timer _timer;

    // Код доступа для API OpenWeatherMap
    private string AppId => _configuration.GetValue<string>("OpenWeatherMapAppId");

    public WeatherService(ILogger<WeatherService> logger, IServiceScopeFactory services,
      IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
      _logger = logger;
      _scope = services;
      _httpClientFactory = httpClientFactory;
      _configuration = configuration;
    }

    public async Task<List<Temperature>> GetCityTemperaturesFromOwmAsync(City city)
    {
      var cityTemperatures = new List<Temperature>();
      var httpClient = _httpClientFactory.CreateClient();
      httpClient.BaseAddress = new Uri("https://api.openweathermap.org");
      var response = await httpClient.GetAsync(
        $"/data/2.5/forecast?q={city.Name}&appid={AppId}&units=metric");
      response.EnsureSuccessStatusCode();

      // Асинхронное чтение потока данных от API OpenWeatherMap
      await using var responseStream = await response.Content.ReadAsStreamAsync();
      var rawWeather = await JsonSerializer.DeserializeAsync<RootObject>(responseStream);
      
      // Конвертация списка температур
      foreach (var list in rawWeather.list)
      {
        var temp = new Temperature()
        {
          City = city,
          Date = DateTimeUtils.UnixTimeStampToDateTime(list.dt),
          Air = (float)list.main.temp
        };

        cityTemperatures.Add(temp);
      }
      return cityTemperatures;
    }

    /// <summary>
    /// Добавить новые температуры в БД
    /// </summary>
    public async void AddNewTemperaturesToDatabase(object state)
    {
      // Объяснение для использования services scope factory: https://www.codeproject.com/Questions/5252916/Error-while-validating-the-service-descriptor-serv
      using var scope = _scope.CreateScope();
      var weatherDbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

      var existingTemperatures = await weatherDbContext.Temperatures.ToListAsync();

      // Забираем список городов из БД
      var uniqueCities = weatherDbContext.Cities
        .AsEnumerable()
        .GroupBy(c => c.Name)
        .Select(c => c.FirstOrDefault())
        .ToList();

      // Определяем новые температуры для городов и добавляем в БД
      foreach (var city in uniqueCities)
      {
        var cityTemperatures = await GetCityTemperaturesFromOwmAsync(city);

        if (cityTemperatures == null) continue;

        // Добавляем новые температурные данные в БД
        city.Temperature ??= new List<Temperature>();

        foreach (var temp in cityTemperatures)
        {
          // Проверяем, что данные по температуре существуют в БД для выбранного города
          if (!existingTemperatures.Any(exTemp =>
                exTemp.Date == temp.Date &&
                Math.Abs(exTemp.Air - temp.Air) < 0.01))
          {
            city.Temperature.Add(temp);
          }
          else
            _logger.LogDebug($"Temperature with data: D:{temp.Date} and T:{temp.Air} " +
                            $"from {temp.City?.Name} Already in database. Will not be added");
        }
      }

      var tempDbWrites = await weatherDbContext.SaveChangesAsync();
      _logger.LogInformation($"Кол-во новых температурных записей в БД." +
                             $"Temperature write operations: {tempDbWrites}");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сервис погоды OWM начал собирать данные...");

      _timer = new Timer(AddNewTemperaturesToDatabase, null, TimeSpan.Zero,
        TimeSpan.FromSeconds(_configuration.GetValue<int>("OpenWeatherMapUpdatePeriodSec")));

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сервис погоды OWM остановился.");
      _timer?.Change(Timeout.Infinite, 30);
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
  }
}

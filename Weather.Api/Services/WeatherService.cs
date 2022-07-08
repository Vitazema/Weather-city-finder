using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weather.Data;

namespace Weather.Api.Services
{
  public class WeatherService : IHostedService, IDisposable
  {
    private int _newTempCount;
    private readonly ILogger _logger;
    private readonly IServiceProvider _scope;
    //private readonly WeatherDbContext _context;
    private Timer? _timer;

    public WeatherService(ILogger<WeatherService> logger, IServiceProvider services)
    {
      _logger = logger;
      _scope = services;
    }


    public void AddNewTemperaturesToDatabase(object? state)
    {
      using (var scope = _scope.CreateScope())
      {
        var _context = scope.ServiceProvider.GetService<WeatherDbContext>();
        var cities = _context.GetCities();

        foreach (var city in cities)
        {
          var cityTemperatures = _context
            .GetTemperatureFromOpenWeatherApiByCity(city).Result;

          if (cityTemperatures == null) continue;
          foreach (var temperature in cityTemperatures)
          {
            _newTempCount += _context.AddTemperatures(city, temperature).Result;
          }
        }

        // Логируем и обнуляем счётчик новых температурных данных
        _logger.LogInformation($"Кол-во новых температурных данных: {_newTempCount}");
        _newTempCount = 0;
      }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сервис погоды OWM начал собирать данные...");

      _timer = new Timer(AddNewTemperaturesToDatabase, null, TimeSpan.Zero,
        TimeSpan.FromSeconds(240));

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Сервис погоды OWM остановился.");
      _timer?.Change(Timeout.Infinite, 0);
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
  }
}

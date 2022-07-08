using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Weather.Domain;
using Weather.UI.Models;
using Weather.UI.Repositories;

namespace Weather.UI.Controllers
{
  public class WeatherController: Controller
  {
    private readonly IWeatherRepository _weatherRepository;

    // DI репозитория с прогнозом погоды от локального WEB API сервиса
    public WeatherController(IWeatherRepository weatherRepository)
    {
      _weatherRepository = weatherRepository;
    }

    /// <summary>
    /// GET: weather/findcity 
    /// </summary>
    public IActionResult FindCity()
    {
      var viewModel = new FindValidator();
      return View(viewModel);
    }

    /// <summary>
    /// Поиск погоды в указанном городе
    /// POST: weather/findcity
    /// </summary>
    /// <param name="model">Модель проверки названия города</param>
    [HttpPost]
    public IActionResult FindCity(FindValidator model)
    {
      // Проверка условий, при котором API вернёт данные погоды для города
      if (ModelState.IsValid)
      {
        return RedirectToAction("City", "Weather", new { city = model.CityName });
      }
      return View(model);
    }

    /// <summary>
    /// Забрать данные с локального WEB API, чтобы получить прогноз погоды на главной странице
    /// </summary>
    /// <param name="city">Название города</param>
    public async Task<IActionResult> City(string city)
    {
      var weatherResult = await _weatherRepository.GetWeatherAsync(city);
      var viewModel = new City();

      if (weatherResult != null)
      {
        viewModel.Name = city;
        viewModel.Temperature = weatherResult;
      }
      return View(viewModel);
    }

    public IActionResult Error()
    {
      return View(new ErrorViewModel() { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}

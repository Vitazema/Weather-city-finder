using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Data;
using Weather.Domain;

namespace Weather.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WeatherController : ControllerBase
  {

    private readonly WeatherDbContext _context;

    public WeatherController(WeatherDbContext context)
    {
      _context = context;
    }

    // GET: api/weather/city
    [HttpGet("city/{cityName}")]
    public async Task<ActionResult<IEnumerable<Temperature>>> GetWeatherByCityName(string cityName)
    {
      var temperatures = await _context.GetWeatherByCityNameAsync(cityName);
      if (temperatures == null)
        return NotFound();
      return temperatures;
    }
  }
}

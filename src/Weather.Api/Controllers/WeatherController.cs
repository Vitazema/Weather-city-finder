﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Weather.Data;
using Weather.Domain;

namespace Weather.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WeatherController : ControllerBase
  {
    private readonly WeatherDbContext _context;
    //private readonly IWeatherRepository _repository;

    public WeatherController(WeatherDbContext context
      //, IWeatherRepository repository
      )
    {
      _context = context;
      //_repository = repository;
    }

    // GET: api/weather/city
    [HttpGet("city/{cityName}")]
    public async Task<List<Temperature>> GetWeatherByCityName(string cityName)
    {
      return await _context.Temperatures
        .Where(t => t.City.Name == cityName)
        .OrderBy(t => t.Date)
        .ToListAsync();
    }
  }
}

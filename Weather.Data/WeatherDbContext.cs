using Microsoft.EntityFrameworkCore;
using Weather.Domain;

namespace Weather.Data
{ public class WeatherDbContext : DbContext
  {
    public DbSet<City> Cities { get; set; }
    public DbSet<Temperature> Temperatures { get; set; }

    public WeatherDbContext() { }

    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
  }
}

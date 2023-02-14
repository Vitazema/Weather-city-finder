using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Domain;

namespace Weather.UI.Repositories
{
  public interface IWeatherRepository
  {
    Task<List<Temperature>> GetWeatherAsync(string cityName);
  }
}

using System.Collections.Generic;

namespace Weather.Domain
{

  /// <summary>
  /// Классы-помощники для десериализации полученного ответа с Openweatherapi
  /// </summary>
  public class RootObject
  {
    public string cod { get; set; }
    public int message { get; set; }
    public int cnt { get; set; }
    public List<List> list { get; set; }

    public City city { get; set; }
  }

  public class Main
  {
    public double temp { get; set; }
  }

  public class List
  {
    public int dt { get; set; }
    public Main main { get; set; }
  }
}

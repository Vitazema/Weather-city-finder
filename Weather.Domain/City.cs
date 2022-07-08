using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Weather.Domain
{
  public class City
  {
    [Key]
    public int Id { get; set; }

    [Display(Name="Город:")]
    public string Name { get; set; }

    [Display(Name="Температура:")]
    public List<Temperature> Temperature { get; set; } = new List<Temperature>();

  }
}

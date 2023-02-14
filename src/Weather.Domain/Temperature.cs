using System;
using System.ComponentModel.DataAnnotations;

namespace Weather.Domain
{
  public class Temperature
  {
    [Key]
    public int Id { get; set; }

    [Display(Name = "Дата:")]
    public DateTime Date { get; set; }

    [Display(Name = "Город:")]
    public City City { get; set; }
    public int CityId { get; set; }

    [Display(Name = "Температура:")]
    public float Air { get; set; }
  }
}
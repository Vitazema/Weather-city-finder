using System.ComponentModel.DataAnnotations;

namespace Weather.UI.Models
{
  public class FindValidator
  {
    // Проверка названия города
    [Required(ErrorMessage = "Введите название города")]
    [RegularExpression("^[A-Za-zА-Яа-я-]+$", ErrorMessage = "Только буквы без пробелов")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Используйте от 2 до 20 букв в названии города")]
    [Display(Name="Название города")]
    public string CityName { get; set; }
  }
}

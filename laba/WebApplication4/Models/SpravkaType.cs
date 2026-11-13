using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название вида справки обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Название от 2 до 50 символов")]
    public string Name { get; set; } = "";

    [Range(0, 1000000, ErrorMessage = "Цена не может быть отрицательной")]
    public decimal Price { get; set; }

    [StringLength(100, ErrorMessage = "Срок действия — до 100 символов")]
    public string Date { get; set; } = ""; //мне лень делать datetime
}

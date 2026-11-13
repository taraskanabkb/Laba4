namespace MedicalApi.Models;

using System.ComponentModel.DataAnnotations;

public class Priem
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PatientId должен быть положительным")]
    public int PatientId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "SpravkaId должен быть положительным")]
    public int SpravkaId { get; set; }

    [Required(ErrorMessage = "Дата приёма обязательна")]
    [StringLength(30, ErrorMessage = "Дата — до 30 символов")]
    public string Date { get; set; } = "";
    public string Doctor { get; set; } = "";
}
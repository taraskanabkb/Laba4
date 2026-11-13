namespace MedicalApi.Models;

using System.ComponentModel.DataAnnotations;

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя от 2 до 50 символов")]
    public string Name { get; set; } = "";

    public DateOnly BirthDate { get; set; }

    [RegularExpression("^(A|B|AB|O)[+-]$",
        ErrorMessage = "Группа крови: A+, A-, B+, B-, AB+, AB-, O+, O-")]
    public string BloodType { get; set; } = "";

    [Range(0, 150, ErrorMessage = "Возраст должен быть от 0 до 150")]
    public int Old { get; set; }

    [Required]
    [RegularExpression(@"^\d{10,12}$",
            ErrorMessage = "ИНН — только цифры, 10 или 12 знаков")]
    public string inn { get; set; } = "";
    public string? Email { get; set; }
}
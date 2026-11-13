using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class Spravka
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int PatientId { get; set; }

    public decimal Price { get; set; }
    
    public int SpravkaTypeId { get; set; }
}

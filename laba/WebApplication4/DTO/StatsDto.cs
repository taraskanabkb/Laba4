namespace MedicalApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class StatsDto
{
    [JsonPropertyName("spravki_count")]
    public int SpravkiCount { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}
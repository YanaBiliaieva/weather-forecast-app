using System.ComponentModel.DataAnnotations;

namespace Server.Application.Contracts.Requests;

public class UpdateWeatherForecastRequest
{
    [Required]
    public DateTime? Date { get; set; }

    [Required]
    [Range(-100, 100)]
    public decimal? TemperatureC { get; set; }

    [Required]
    [StringLength(256)]
    public string? Summary { get; set; }

    [Required]
    [StringLength(128)]
    public string? City { get; set; }
}

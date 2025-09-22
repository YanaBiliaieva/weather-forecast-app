using Server.Domain.Abstractions;
using Server.Domain.Events;

namespace Server.Domain.Entities;

public class WeatherForecast : Entity
{
    private WeatherForecast()
    {
        // Required by EF Core
    }

    public WeatherForecast(DateTime date, decimal temperatureC, string summary, string city)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
        City = city;

        AddDomainEvent(new WeatherForecastCreatedEvent(Id, date, city));
    }

    public DateTime Date { get; private set; }

    public decimal TemperatureC { get; private set; }

    public string Summary { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public decimal TemperatureF => Math.Round(32 + (TemperatureC / 0.5556m), 1);

    public void Update(DateTime date, decimal temperatureC, string summary, string city)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
        City = city;

        AddDomainEvent(new WeatherForecastUpdatedEvent(Id, date, city));
    }

    public void MarkDeleted()
    {
        AddDomainEvent(new WeatherForecastDeletedEvent(Id));
    }
}

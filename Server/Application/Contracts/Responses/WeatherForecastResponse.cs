namespace Server.Application.Contracts.Responses;

public record WeatherForecastResponse(
    Guid Id,
    DateTime Date,
    decimal TemperatureC,
    decimal TemperatureF,
    string Summary,
    string City);

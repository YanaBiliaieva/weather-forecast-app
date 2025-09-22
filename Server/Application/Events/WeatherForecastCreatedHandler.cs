using Microsoft.Extensions.Logging;
using Server.Domain.Abstractions;
using Server.Domain.Events;

namespace Server.Application.Events;

public class WeatherForecastCreatedHandler : IDomainEventHandler<WeatherForecastCreatedEvent>
{
    private readonly ILogger<WeatherForecastCreatedHandler> _logger;

    public WeatherForecastCreatedHandler(ILogger<WeatherForecastCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(WeatherForecastCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Weather forecast {ForecastId} created for {City} on {Date}", domainEvent.ForecastId, domainEvent.City, domainEvent.Date);
        return Task.CompletedTask;
    }
}

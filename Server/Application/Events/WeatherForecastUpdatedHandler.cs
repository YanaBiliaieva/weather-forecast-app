using Microsoft.Extensions.Logging;
using Server.Domain.Abstractions;
using Server.Domain.Events;

namespace Server.Application.Events;

public class WeatherForecastUpdatedHandler : IDomainEventHandler<WeatherForecastUpdatedEvent>
{
    private readonly ILogger<WeatherForecastUpdatedHandler> _logger;

    public WeatherForecastUpdatedHandler(ILogger<WeatherForecastUpdatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(WeatherForecastUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Weather forecast {ForecastId} updated for {City} on {Date}", domainEvent.ForecastId, domainEvent.City, domainEvent.Date);
        return Task.CompletedTask;
    }
}

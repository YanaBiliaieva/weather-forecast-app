using Microsoft.Extensions.Logging;
using Server.Domain.Abstractions;
using Server.Domain.Events;

namespace Server.Application.Events;

public class WeatherForecastDeletedHandler : IDomainEventHandler<WeatherForecastDeletedEvent>
{
    private readonly ILogger<WeatherForecastDeletedHandler> _logger;

    public WeatherForecastDeletedHandler(ILogger<WeatherForecastDeletedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(WeatherForecastDeletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Weather forecast {ForecastId} removed", domainEvent.ForecastId);
        return Task.CompletedTask;
    }
}

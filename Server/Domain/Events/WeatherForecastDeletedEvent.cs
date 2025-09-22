using Server.Domain.Abstractions;

namespace Server.Domain.Events;

public record WeatherForecastDeletedEvent(Guid ForecastId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

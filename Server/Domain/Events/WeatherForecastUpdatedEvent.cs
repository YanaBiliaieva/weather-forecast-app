using Server.Domain.Abstractions;

namespace Server.Domain.Events;

public record WeatherForecastUpdatedEvent(Guid ForecastId, DateTime Date, string City) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

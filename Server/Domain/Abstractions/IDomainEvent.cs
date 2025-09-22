namespace Server.Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

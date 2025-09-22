using Server.Domain.Abstractions;

namespace Server.Infrastructure.Events;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var handlerType = typeof(IEnumerable<>).MakeGenericType(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));
            var handlers = (IEnumerable<object>?)_serviceProvider.GetService(handlerType);

            if (handlers is null)
            {
                continue;
            }

            foreach (var handler in handlers)
            {
                var method = handler.GetType().GetMethod("HandleAsync");
                if (method is null)
                {
                    continue;
                }

                var task = (Task?)method.Invoke(handler, new object[] { domainEvent, cancellationToken });
                if (task is not null)
                {
                    await task.ConfigureAwait(false);
                }
            }
        }
    }
}

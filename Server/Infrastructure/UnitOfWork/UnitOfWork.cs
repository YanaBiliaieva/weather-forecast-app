using Microsoft.EntityFrameworkCore;
using Server.Domain.Abstractions;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IEventDispatcher _eventDispatcher;

    public UnitOfWork(AppDbContext context, IEventDispatcher eventDispatcher)
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        var result = await _context.SaveChangesAsync(cancellationToken);

        await _eventDispatcher.DispatchAsync(domainEvents, cancellationToken);

        foreach (var entry in domainEntities)
        {
            entry.Entity.ClearDomainEvents();
        }

        return result;
    }
}

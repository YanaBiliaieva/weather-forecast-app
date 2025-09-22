using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.Repositories;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly AppDbContext _context;

    public WeatherForecastRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WeatherForecasts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default)
    {
        await _context.WeatherForecasts.AddAsync(forecast, cancellationToken);
    }

    public void Remove(WeatherForecast forecast)
    {
        _context.WeatherForecasts.Remove(forecast);
    }
}

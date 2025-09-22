using Server.Domain.Entities;

namespace Server.Domain.Repositories;

public interface IWeatherForecastRepository
{
    Task<IReadOnlyList<WeatherForecast>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<WeatherForecast?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(WeatherForecast forecast, CancellationToken cancellationToken = default);

    void Remove(WeatherForecast forecast);
}

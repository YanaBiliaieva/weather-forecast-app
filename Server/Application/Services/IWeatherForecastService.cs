using Server.Application.Contracts.Requests;
using Server.Application.Contracts.Responses;

namespace Server.Application.Services;

public interface IWeatherForecastService
{
    Task<IReadOnlyList<WeatherForecastResponse>> GetAsync(CancellationToken cancellationToken = default);

    Task<WeatherForecastResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(CreateWeatherForecastRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, UpdateWeatherForecastRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

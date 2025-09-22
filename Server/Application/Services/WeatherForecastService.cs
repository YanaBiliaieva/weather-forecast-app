using Server.Application.Contracts.Requests;
using Server.Application.Contracts.Responses;
using Server.Domain.Abstractions;
using Server.Domain.Entities;
using Server.Domain.Repositories;

namespace Server.Application.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public WeatherForecastService(IWeatherForecastRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<WeatherForecastResponse>> GetAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await _repository.GetAllAsync(cancellationToken);
        return forecasts.Select(MapToResponse).ToList();
    }

    public async Task<WeatherForecastResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var forecast = await _repository.GetByIdAsync(id, cancellationToken);
        return forecast is null ? null : MapToResponse(forecast);
    }

    public async Task<Guid> CreateAsync(CreateWeatherForecastRequest request, CancellationToken cancellationToken = default)
    {
        var forecast = new WeatherForecast(
            request.Date!.Value,
            request.TemperatureC!.Value,
            request.Summary!,
            request.City!);

        await _repository.AddAsync(forecast, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return forecast.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateWeatherForecastRequest request, CancellationToken cancellationToken = default)
    {
        var forecast = await _repository.GetByIdAsync(id, cancellationToken);
        if (forecast is null)
        {
            return false;
        }

        forecast.Update(
            request.Date!.Value,
            request.TemperatureC!.Value,
            request.Summary!,
            request.City!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var forecast = await _repository.GetByIdAsync(id, cancellationToken);
        if (forecast is null)
        {
            return false;
        }

        forecast.MarkDeleted();
        _repository.Remove(forecast);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static WeatherForecastResponse MapToResponse(WeatherForecast forecast)
    {
        return new WeatherForecastResponse(
            forecast.Id,
            forecast.Date,
            forecast.TemperatureC,
            forecast.TemperatureF,
            forecast.Summary,
            forecast.City);
    }
}

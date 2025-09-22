using Moq;
using Server.Application.Contracts.Requests;
using Server.Application.Services;
using Server.Domain.Abstractions;
using Server.Domain.Entities;
using Server.Domain.Repositories;

namespace Server.UnitTests;

public class WeatherForecastServiceTests
{
    private readonly Mock<IWeatherForecastRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly WeatherForecastService _service;

    public WeatherForecastServiceTests()
    {
        _repositoryMock = new Mock<IWeatherForecastRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _service = new WeatherForecastService(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_AddsForecastAndReturnsId()
    {
        // Arrange
        var request = new CreateWeatherForecastRequest
        {
            Date = DateTime.Today,
            TemperatureC = 12.3m,
            Summary = "Crisp morning",
            City = "Oslo",
        };

        WeatherForecast? addedForecast = null;
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<WeatherForecast>(), It.IsAny<CancellationToken>()))
            .Callback<WeatherForecast, CancellationToken>((forecast, _) => addedForecast = forecast)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        Assert.NotNull(addedForecast);
        Assert.Equal(request.City, addedForecast!.City);
        Assert.Equal(request.Summary, addedForecast.Summary);
        Assert.Equal(request.Date, addedForecast.Date);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<WeatherForecast>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenForecastNotFound()
    {
        // Arrange
        var request = new UpdateWeatherForecastRequest
        {
            Date = DateTime.Today,
            TemperatureC = 18.4m,
            Summary = "Sunny",
            City = "Lisbon",
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherForecast?)null);

        // Act
        var updated = await _service.UpdateAsync(Guid.NewGuid(), request);

        // Assert
        Assert.False(updated);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ChangesValues_WhenForecastExists()
    {
        // Arrange
        var forecast = new WeatherForecast(DateTime.Today, 10m, "Cloudy", "Paris");
        var request = new UpdateWeatherForecastRequest
        {
            Date = DateTime.Today.AddDays(1),
            TemperatureC = 21.5m,
            Summary = "Warm",
            City = "Nice",
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(forecast.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecast);

        // Act
        var updated = await _service.UpdateAsync(forecast.Id, request);

        // Assert
        Assert.True(updated);
        Assert.Equal(request.City, forecast.City);
        Assert.Equal(request.Summary, forecast.Summary);
        Assert.Equal(request.Date, forecast.Date);
        Assert.Equal(request.TemperatureC, forecast.TemperatureC);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RemovesForecast_WhenItExists()
    {
        // Arrange
        var forecast = new WeatherForecast(DateTime.Today, 5m, "Snow", "Reykjavik");
        _repositoryMock
            .Setup(r => r.GetByIdAsync(forecast.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forecast);

        // Act
        var deleted = await _service.DeleteAsync(forecast.Id);

        // Assert
        Assert.True(deleted);
        _repositoryMock.Verify(r => r.Remove(forecast), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

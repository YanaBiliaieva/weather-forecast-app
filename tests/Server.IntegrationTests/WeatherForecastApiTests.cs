using System.Net.Http.Json;

namespace Server.IntegrationTests;

public class WeatherForecastApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WeatherForecastApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ForecastCrudLifecycle_WorksAsExpected()
    {
        var createPayload = new
        {
            date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
            temperatureC = 14.5m,
            summary = "Pleasant",
            city = "Berlin",
        };

        var createResponse = await _client.PostAsJsonAsync("/api/WeatherForecast", createPayload);
        var createBody = await createResponse.Content.ReadAsStringAsync();
        Assert.True(createResponse.IsSuccessStatusCode, createBody);
        Assert.NotNull(createResponse.Headers.Location);

        var list = await _client.GetFromJsonAsync<List<ApiForecast>>("/api/WeatherForecast");
        Assert.NotNull(list);
        var created = Assert.Single(list!, x => x.City == createPayload.city);

        var updatePayload = new
        {
            date = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-dd"),
            temperatureC = 22.1m,
            summary = "Warm",
            city = "Berlin",
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/WeatherForecast/{created.Id}", updatePayload);
        Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetFromJsonAsync<ApiForecast>($"/api/WeatherForecast/{created.Id}");
        Assert.NotNull(getResponse);
        Assert.Equal(updatePayload.summary, getResponse!.Summary);
        Assert.Equal(updatePayload.temperatureC, getResponse.TemperatureC);

        var deleteResponse = await _client.DeleteAsync($"/api/WeatherForecast/{created.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var finalList = await _client.GetFromJsonAsync<List<ApiForecast>>("/api/WeatherForecast");
        Assert.NotNull(finalList);
        Assert.DoesNotContain(finalList!, x => x.Id == created.Id);
    }

    private sealed record ApiForecast(Guid Id, DateTime Date, decimal TemperatureC, decimal TemperatureF, string Summary, string City);
}

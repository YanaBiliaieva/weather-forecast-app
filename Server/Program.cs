using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Server.Application.Events;
using Server.Application.Services;
using Server.Domain.Abstractions;
using Server.Domain.Events;
using Server.Domain.Repositories;
using Server.Extensions;
using Server.Infrastructure.Events;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new DryIocServiceProviderFactory());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        var origins = builder.Configuration
            .GetSection("Frontend:AllowedOrigins")
            .Get<string[]>() ?? new[] { "http://localhost:5173" };

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var provider = configuration.GetValue<string>("Database:Provider") ?? "SqlServer";

    if (string.Equals(provider, "InMemory", StringComparison.OrdinalIgnoreCase))
    {
        var dbName = configuration.GetValue<string>("Database:InMemoryName") ?? "WeatherTests";
        options.UseInMemoryDatabase(dbName);
    }
    else if (string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var connectionString = configuration.GetConnectionString("SqliteConnection") ?? "DataSource=weather-tests.db";
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
});

builder.Services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<WeatherForecastCreatedEvent>, WeatherForecastCreatedHandler>();
builder.Services.AddScoped<IDomainEventHandler<WeatherForecastUpdatedEvent>, WeatherForecastUpdatedHandler>();
builder.Services.AddScoped<IDomainEventHandler<WeatherForecastDeletedEvent>, WeatherForecastDeletedHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrationsAsync();

app.UseCors("ClientPolicy");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;

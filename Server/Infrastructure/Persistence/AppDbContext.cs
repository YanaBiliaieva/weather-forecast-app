using Microsoft.EntityFrameworkCore;
using Server.Domain.Abstractions;
using Server.Domain.Entities;

namespace Server.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.City)
                  .HasMaxLength(128)
                  .IsRequired();
            entity.Property(x => x.Summary)
                  .HasMaxLength(256)
                  .IsRequired();
            entity.Property(x => x.TemperatureC)
                  .HasColumnType("decimal(5,2)");
            entity.Property(x => x.Date)
                  .IsRequired();
        });
    }
}

using Microsoft.EntityFrameworkCore;
using SurveySystem.Domain.Entities;
namespace SurveySystem.Infrastructure.Persistance;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<TaxZone> TaxZones { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<TaxZone>(builder =>
        {
            builder.HasKey(tz => tz.Id);
            builder.Property(tz => tz.StateRate).HasPrecision(10, 5);
            builder.Property(tz => tz.CountyRate).HasPrecision(10, 5);
            builder.Property(tz => tz.CityRate).HasPrecision(10, 5);
            builder.Property(tz => tz.SpecialRates).HasPrecision(10, 5);

            builder.Property(tz => tz.Boundary).HasColumnType("geometry");
        });
        
        modelBuilder.Entity<Order>(builder =>
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.ExternalId).IsRequired(false);

            builder.Property(o => o.Subtotal).HasPrecision(18, 2);
            builder.Property(o => o.TaxAmount).HasPrecision(18, 2);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 2);

            builder.Property(o => o.CompositeTaxRate).HasPrecision(10, 5);

            builder.OwnsOne(o => o.Breakdown, b =>
            {
                b.Property(x => x.StateRate).HasColumnName("StateRate").HasPrecision(10, 5);
                b.Property(x => x.CountyRate).HasColumnName("CountyRate").HasPrecision(10, 5);
                b.Property(x => x.CityRate).HasColumnName("CityRate").HasPrecision(10, 5);
                b.Property(x => x.SpecialRates).HasColumnName("SpecialRates").HasPrecision(10, 5);
            });

            builder.Property(o => o.Jurisdictions)
                .HasColumnType("jsonb");
        });
    }
}

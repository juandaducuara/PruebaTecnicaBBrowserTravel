using Microsoft.EntityFrameworkCore;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;

namespace OutletRentalCars.Infrastructure.Persistence.MySQL;

public class AppDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Location> Locations => Set<Location>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Brand).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Model).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LicensePlate).HasMaxLength(20).IsRequired();
            entity.Property(e => e.VehicleTypeId).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Location)
                .WithMany(l => l.Vehicles)
                .HasForeignKey(e => e.LocationId);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>();

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.Reservations)
                .HasForeignKey(e => e.VehicleId);

            entity.Ignore(e => e.DomainEvents);
        });

        // Ignorar DomainEvents en todas las entidades base
        modelBuilder.Entity<Location>().Ignore(e => e.DomainEvents);
        modelBuilder.Entity<Vehicle>().Ignore(e => e.DomainEvents);
    }
}

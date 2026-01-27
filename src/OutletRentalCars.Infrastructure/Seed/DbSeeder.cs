using MongoDB.Driver;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;
using OutletRentalCars.Infrastructure.Persistence.MongoDB;
using OutletRentalCars.Infrastructure.Persistence.MySQL;

namespace OutletRentalCars.Infrastructure.Seed;

public static class DbSeeder
{
    // IDs autoincrementales asignados por EF Core
    // Bogotá = 1, Medellín = 2, Cali = 3
    public const int LocationBogotaId = 1;
    public const int LocationMedellinId = 2;
    public const int LocationCaliId = 3;

    // Vehicle IDs: Toyota=1, Chevrolet=2, Renault=3, Mazda=4, Kia=5, Nissan=6
    public const int Vehicle1Id = 1;
    public const int Vehicle2Id = 2;
    public const int Vehicle3Id = 3;
    public const int Vehicle4Id = 4;
    public const int Vehicle5Id = 5;
    public const int Vehicle6Id = 6;

    public static async Task SeedMySqlAsync(AppDbContext context)
    {
        if (context.Locations.Any())
            return;

        var bogota = new Location("Bogotá - El Dorado", "Colombia");
        var medellin = new Location("Medellín - José María Córdova", "Colombia");
        var cali = new Location("Cali - Alfonso Bonilla Aragón", "Colombia");

        context.Locations.AddRange(bogota, medellin, cali);
        await context.SaveChangesAsync();

        var vehicles = new List<Vehicle>
        {
            new("Toyota", "Corolla", 2023, "ABC-123", VehicleStatus.Available, "sedan", "Colombia", bogota.Id),
            new("Chevrolet", "Tracker", 2024, "DEF-456", VehicleStatus.Available, "suv", "Colombia", bogota.Id),
            new("Renault", "Kwid", 2023, "GHI-789", VehicleStatus.Maintenance, "economy", "Colombia", bogota.Id),
            new("Mazda", "CX-5", 2024, "JKL-012", VehicleStatus.Available, "suv", "Colombia", medellin.Id),
            new("Kia", "Sportage", 2024, "MNO-345", VehicleStatus.Available, "sport", "Colombia", medellin.Id),
            new("Nissan", "Versa", 2023, "PQR-678", VehicleStatus.Available, "sedan", "Colombia", cali.Id)
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        // Una reserva activa para el primer vehículo (Toyota Corolla) en un rango específico
        var reservation = Reservation.Create(
            vehicles[0].Id, bogota.Id, bogota.Id,
            new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 3, 5, 10, 0, 0, DateTimeKind.Utc),
            "Juan Pérez");

        reservation.ClearDomainEvents();
        context.Reservations.Add(reservation);

        await context.SaveChangesAsync();
    }

    public static async Task SeedMongoDbAsync(MongoDbContext context)
    {
        var marketsCount = await context.Markets.CountDocumentsAsync(Builders<Market>.Filter.Empty);
        if (marketsCount > 0)
            return;

        var markets = new List<Market>
        {
            new() { Id = "mkt-col", Country = "Colombia", Currency = "COP", IsActive = true },
            new() { Id = "mkt-mex", Country = "Mexico", Currency = "MXN", IsActive = false }
        };

        await context.Markets.InsertManyAsync(markets);

        var vehicleTypes = new List<VehicleType>
        {
            new() { Id = "sedan", Name = "Sedán", Description = "Vehículo de 4 puertas, ideal para ciudad" },
            new() { Id = "suv", Name = "SUV", Description = "Vehículo utilitario deportivo" },
            new() { Id = "economy", Name = "Económico", Description = "Vehículo compacto de bajo consumo" },
            new() { Id = "sport", Name = "Deportivo", Description = "Vehículo de alto rendimiento" }
        };

        await context.VehicleTypes.InsertManyAsync(vehicleTypes);
    }
}

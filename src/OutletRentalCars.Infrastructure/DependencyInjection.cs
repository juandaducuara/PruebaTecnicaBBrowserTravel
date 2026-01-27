using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Infrastructure.Persistence.MongoDB;
using OutletRentalCars.Infrastructure.Persistence.MySQL;
using OutletRentalCars.Infrastructure.Repositories;

namespace OutletRentalCars.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MySQL
        var mysqlConnection = configuration.GetConnectionString("MySQL")
            ?? "Server=localhost;Database=outletrentalcars;User=root;";

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection)));

        // MongoDB
        var mongoConnection = configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://localhost:27017";
        var mongoDatabase = configuration.GetValue<string>("MongoDB:DatabaseName") ?? "OutletRentalCarsDb";

        services.AddSingleton(new MongoDbContext(mongoConnection, mongoDatabase));

        // Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IMarketRepository, MarketRepository>();

        return services;
    }
}

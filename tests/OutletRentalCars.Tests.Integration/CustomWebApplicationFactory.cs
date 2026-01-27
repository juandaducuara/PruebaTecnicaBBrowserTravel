using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Infrastructure.Persistence.MongoDB;
using OutletRentalCars.Infrastructure.Persistence.MySQL;
using Moq;

namespace OutletRentalCars.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el DbContext real de MySQL
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Remover el MongoDbContext real
            var mongoDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(MongoDbContext));
            if (mongoDescriptor != null)
                services.Remove(mongoDescriptor);

            // Registrar MongoDbContext dummy (no se usa, las queries de Mongo van por mock)
            services.AddSingleton(new MongoDbContext("mongodb://localhost:27017", "TestDb_Dummy"));

            // Usar InMemory con nombre fijo para que el seed de Program.cs persista
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // Mock de MarketRepository para no depender de MongoDB
            var marketRepoMock = new Mock<IMarketRepository>();
            marketRepoMock.Setup(x => x.IsMarketActiveAsync("Colombia", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            marketRepoMock.Setup(x => x.IsMarketActiveAsync("Mexico", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var marketDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMarketRepository));
            if (marketDescriptor != null)
                services.Remove(marketDescriptor);

            services.AddScoped<IMarketRepository>(_ => marketRepoMock.Object);
        });

        builder.UseEnvironment("Testing");
    }
}

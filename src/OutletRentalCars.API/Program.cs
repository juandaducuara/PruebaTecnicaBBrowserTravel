using OutletRentalCars.Application.Reservations.EventHandlers;
using OutletRentalCars.Infrastructure;
using OutletRentalCars.Infrastructure.Persistence.MongoDB;
using OutletRentalCars.Infrastructure.Persistence.MySQL;
using OutletRentalCars.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// MediatR (escanea handlers en Application)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<VehicleReservedEventHandler>());

// Infrastructure (MySQL, MongoDB, repositorios)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OutletRentalCars API",
        Version = "v1",
        Description = "API de búsqueda y reserva de vehículos para Outlet Rental Cars"
    });
});

var app = builder.Build();

// Seed de datos al arrancar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    await DbSeeder.SeedMySqlAsync(dbContext);

    try
    {
        var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        await DbSeeder.SeedMongoDbAsync(mongoContext);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "No se pudo conectar a MongoDB para el seed. Esto es esperado en entorno de testing.");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OutletRentalCars API v1"));

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Necesario para WebApplicationFactory en tests de integración
public partial class Program { }

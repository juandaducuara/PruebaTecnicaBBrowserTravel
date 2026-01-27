using MongoDB.Driver;
using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Infrastructure.Persistence.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Market> Markets => _database.GetCollection<Market>("Markets");
    public IMongoCollection<VehicleType> VehicleTypes => _database.GetCollection<VehicleType>("VehicleTypes");
}

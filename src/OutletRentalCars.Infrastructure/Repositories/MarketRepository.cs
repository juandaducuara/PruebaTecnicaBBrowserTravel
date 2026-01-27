using MongoDB.Driver;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Infrastructure.Persistence.MongoDB;

namespace OutletRentalCars.Infrastructure.Repositories;

public class MarketRepository : IMarketRepository
{
    private readonly MongoDbContext _context;

    public MarketRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Market?> GetByCountryAsync(string country, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Market>.Filter.Eq(m => m.Country, country);
        return await _context.Markets.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsMarketActiveAsync(string country, CancellationToken cancellationToken = default)
    {
        var market = await GetByCountryAsync(country, cancellationToken);
        return market is { IsActive: true };
    }
}

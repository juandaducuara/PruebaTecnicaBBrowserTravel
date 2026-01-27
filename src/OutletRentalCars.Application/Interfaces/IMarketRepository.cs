using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Application.Interfaces;

public interface IMarketRepository
{
    Task<Market?> GetByCountryAsync(string country, CancellationToken cancellationToken = default);
    Task<bool> IsMarketActiveAsync(string country, CancellationToken cancellationToken = default);
}

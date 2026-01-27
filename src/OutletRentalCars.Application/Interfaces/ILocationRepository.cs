using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Application.Interfaces;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

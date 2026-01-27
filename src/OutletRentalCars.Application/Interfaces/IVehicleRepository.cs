using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Application.Interfaces;

public interface IVehicleRepository
{
    Task<List<Vehicle>> SearchAvailableVehiclesAsync(
        int pickupLocationId,
        DateTime pickupDate,
        DateTime dropoffDate,
        string? vehicleType = null,
        CancellationToken cancellationToken = default);
}

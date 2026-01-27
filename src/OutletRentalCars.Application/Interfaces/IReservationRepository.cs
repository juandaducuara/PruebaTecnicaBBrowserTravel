using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Application.Interfaces;

public interface IReservationRepository
{
    Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingReservationAsync(int vehicleId, DateTime pickupDate, DateTime dropoffDate, CancellationToken cancellationToken = default);
}

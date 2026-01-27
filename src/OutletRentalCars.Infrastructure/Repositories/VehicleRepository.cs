using Microsoft.EntityFrameworkCore;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;
using OutletRentalCars.Infrastructure.Persistence.MySQL;

namespace OutletRentalCars.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vehicle>> SearchAvailableVehiclesAsync(
        int pickupLocationId,
        DateTime pickupDate,
        DateTime dropoffDate,
        string? vehicleType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Vehicles
            .Include(v => v.Location)
            .Include(v => v.Reservations)
            .Where(v => v.LocationId == pickupLocationId)
            .Where(v => v.Status == VehicleStatus.Available)
            .Where(v => !v.Reservations.Any(r =>
                r.Status == ReservationStatus.Active &&
                r.PickupDate < dropoffDate &&
                r.DropoffDate > pickupDate));

        if (!string.IsNullOrWhiteSpace(vehicleType))
            query = query.Where(v => v.VehicleTypeId == vehicleType);

        return await query.ToListAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;
using OutletRentalCars.Infrastructure.Persistence.MySQL;

namespace OutletRentalCars.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task<bool> HasOverlappingReservationAsync(
        int vehicleId,
        DateTime pickupDate,
        DateTime dropoffDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .AnyAsync(r =>
                r.VehicleId == vehicleId &&
                r.Status == ReservationStatus.Active &&
                r.PickupDate < dropoffDate &&
                r.DropoffDate > pickupDate,
                cancellationToken);
    }
}

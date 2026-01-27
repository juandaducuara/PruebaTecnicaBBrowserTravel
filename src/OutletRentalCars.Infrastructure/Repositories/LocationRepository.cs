using Microsoft.EntityFrameworkCore;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Infrastructure.Persistence.MySQL;

namespace OutletRentalCars.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}

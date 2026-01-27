using MediatR;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Application.Vehicles.DTOs;

namespace OutletRentalCars.Application.Vehicles.Queries;

public class SearchVehiclesQueryHandler : IRequestHandler<SearchVehiclesQuery, List<VehicleSearchResultDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IMarketRepository _marketRepository;

    public SearchVehiclesQueryHandler(
        IVehicleRepository vehicleRepository,
        ILocationRepository locationRepository,
        IMarketRepository marketRepository)
    {
        _vehicleRepository = vehicleRepository;
        _locationRepository = locationRepository;
        _marketRepository = marketRepository;
    }

    public async Task<List<VehicleSearchResultDto>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        if (request.PickupDate >= request.DropoffDate)
            throw new ArgumentException("La fecha de recogida debe ser anterior a la fecha de devolución.");

        if (request.PickupDate < DateTime.UtcNow)
            throw new ArgumentException("La fecha de recogida no puede ser en el pasado.");

        var pickupLocation = await _locationRepository.GetByIdAsync(request.PickupLocationId, cancellationToken)
            ?? throw new ArgumentException("La localidad de recogida no existe.");

        // Verificar que el mercado esté activo para el país de la localidad
        var marketActive = await _marketRepository.IsMarketActiveAsync(pickupLocation.Country, cancellationToken);
        if (!marketActive)
            return new List<VehicleSearchResultDto>();

        // Buscar vehículos disponibles en la localidad de recogida, sin reservas cruzadas
        var vehicles = await _vehicleRepository.SearchAvailableVehiclesAsync(
            request.PickupLocationId,
            request.PickupDate,
            request.DropoffDate,
            request.VehicleType,
            cancellationToken);

        // Filtrar solo los que pertenecen al mercado del país de la localidad
        var filtered = vehicles
            .Where(v => v.BelongsToMarket(pickupLocation.Country))
            .ToList();

        return filtered.Select(v => new VehicleSearchResultDto
        {
            Id = v.Id,
            Brand = v.Brand,
            Model = v.Model,
            Year = v.Year,
            LicensePlate = v.LicensePlate,
            VehicleType = v.VehicleTypeId,
            LocationName = v.Location?.Name ?? string.Empty,
            Country = v.Country
        }).ToList();
    }
}

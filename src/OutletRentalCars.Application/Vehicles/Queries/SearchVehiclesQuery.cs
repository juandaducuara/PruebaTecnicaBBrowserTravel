using MediatR;
using OutletRentalCars.Application.Vehicles.DTOs;

namespace OutletRentalCars.Application.Vehicles.Queries;

public record SearchVehiclesQuery(
    int PickupLocationId,
    int DropoffLocationId,
    DateTime PickupDate,
    DateTime DropoffDate,
    string? VehicleType = null
) : IRequest<List<VehicleSearchResultDto>>;

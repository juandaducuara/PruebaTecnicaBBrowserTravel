using MediatR;

namespace OutletRentalCars.Application.Reservations.Commands;

public record CreateReservationCommand(
    int VehicleId,
    int PickupLocationId,
    int DropoffLocationId,
    DateTime PickupDate,
    DateTime DropoffDate,
    string CustomerName
) : IRequest<int>;

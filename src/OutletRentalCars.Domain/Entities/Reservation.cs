using OutletRentalCars.Domain.Common;
using OutletRentalCars.Domain.Enums;
using OutletRentalCars.Domain.Events;

namespace OutletRentalCars.Domain.Entities;

public class Reservation : Entity
{
    public int VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = null!;

    public int PickupLocationId { get; private set; }
    public int DropoffLocationId { get; private set; }

    public DateTime PickupDate { get; private set; }
    public DateTime DropoffDate { get; private set; }

    public ReservationStatus Status { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Reservation() { }

    public static Reservation Create(int vehicleId, int pickupLocationId, int dropoffLocationId,
        DateTime pickupDate, DateTime dropoffDate, string customerName)
    {
        var reservation = new Reservation
        {
            VehicleId = vehicleId,
            PickupLocationId = pickupLocationId,
            DropoffLocationId = dropoffLocationId,
            PickupDate = pickupDate,
            DropoffDate = dropoffDate,
            Status = ReservationStatus.Active,
            CustomerName = customerName,
            CreatedAt = DateTime.UtcNow
        };

        reservation.AddDomainEvent(new VehicleReservedEvent(
            reservation.Id, vehicleId, pickupDate, dropoffDate, customerName));

        return reservation;
    }
}

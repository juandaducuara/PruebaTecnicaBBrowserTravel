using OutletRentalCars.Domain.Common;

namespace OutletRentalCars.Domain.Events;

public class VehicleReservedEvent : DomainEvent
{
    public int ReservationId { get; }
    public int VehicleId { get; }
    public DateTime PickupDate { get; }
    public DateTime DropoffDate { get; }
    public string CustomerName { get; }

    public VehicleReservedEvent(int reservationId, int vehicleId,
        DateTime pickupDate, DateTime dropoffDate, string customerName)
    {
        ReservationId = reservationId;
        VehicleId = vehicleId;
        PickupDate = pickupDate;
        DropoffDate = dropoffDate;
        CustomerName = customerName;
    }
}

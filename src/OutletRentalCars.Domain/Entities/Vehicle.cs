using OutletRentalCars.Domain.Common;
using OutletRentalCars.Domain.Enums;

namespace OutletRentalCars.Domain.Entities;

public class Vehicle : Entity
{
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string LicensePlate { get; private set; } = string.Empty;
    public VehicleStatus Status { get; private set; }
    public string VehicleTypeId { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;

    // FK
    public int LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();

    private Vehicle() { }

    public Vehicle(string brand, string model, int year, string licensePlate,
        VehicleStatus status, string vehicleTypeId, string country, int locationId)
    {
        Brand = brand;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
        Status = status;
        VehicleTypeId = vehicleTypeId;
        Country = country;
        LocationId = locationId;
    }

    public bool IsAvailable() => Status == VehicleStatus.Available;

    public bool BelongsToMarket(string country) =>
        Country.Equals(country, StringComparison.OrdinalIgnoreCase);

    public bool HasOverlappingReservation(DateTime pickupDate, DateTime dropoffDate)
    {
        return Reservations.Any(r =>
            r.Status == ReservationStatus.Active &&
            r.PickupDate < dropoffDate &&
            r.DropoffDate > pickupDate);
    }
}

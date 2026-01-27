using OutletRentalCars.Domain.Common;

namespace OutletRentalCars.Domain.Entities;

public class Location : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;

    // EF nav
    public ICollection<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();

    private Location() { }

    public Location(string name, string country)
    {
        Name = name;
        Country = country;
    }
}

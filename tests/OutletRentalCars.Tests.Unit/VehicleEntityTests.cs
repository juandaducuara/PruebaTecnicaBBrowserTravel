using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;

namespace OutletRentalCars.Tests.Unit;

public class VehicleEntityTests
{
    [Fact]
    public void IsAvailable_ReturnsTrue_WhenStatusIsAvailable()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2023, "ABC-123",
            VehicleStatus.Available, "sedan", "Colombia", 1);

        Assert.True(vehicle.IsAvailable());
    }

    [Fact]
    public void IsAvailable_ReturnsFalse_WhenStatusIsMaintenance()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2023, "ABC-123",
            VehicleStatus.Maintenance, "sedan", "Colombia", 1);

        Assert.False(vehicle.IsAvailable());
    }

    [Fact]
    public void BelongsToMarket_ReturnsTrue_WhenCountryMatches()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2023, "ABC-123",
            VehicleStatus.Available, "sedan", "Colombia", 1);

        Assert.True(vehicle.BelongsToMarket("Colombia"));
    }

    [Fact]
    public void BelongsToMarket_ReturnsFalse_WhenCountryDoesNotMatch()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", 2023, "ABC-123",
            VehicleStatus.Available, "sedan", "Colombia", 1);

        Assert.False(vehicle.BelongsToMarket("Mexico"));
    }
}

using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;
using OutletRentalCars.Domain.Events;

namespace OutletRentalCars.Tests.Unit;

public class ReservationEntityTests
{
    [Fact]
    public void Create_ShouldGenerateVehicleReservedEvent()
    {
        var reservation = Reservation.Create(
            1, 1, 2,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3),
            "Test Customer");

        Assert.Single(reservation.DomainEvents);
        Assert.IsType<VehicleReservedEvent>(reservation.DomainEvents[0]);
    }

    [Fact]
    public void Create_ShouldSetStatusToActive()
    {
        var reservation = Reservation.Create(
            1, 1, 2,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3),
            "Test Customer");

        Assert.Equal(ReservationStatus.Active, reservation.Status);
    }

    [Fact]
    public void Create_ShouldSetCorrectCustomerName()
    {
        var reservation = Reservation.Create(
            1, 1, 2,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3),
            "María García");

        Assert.Equal("María García", reservation.CustomerName);
    }
}

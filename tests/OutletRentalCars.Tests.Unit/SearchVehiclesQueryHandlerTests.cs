using Moq;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Application.Vehicles.Queries;
using OutletRentalCars.Domain.Entities;
using OutletRentalCars.Domain.Enums;

namespace OutletRentalCars.Tests.Unit;

public class SearchVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepoMock;
    private readonly Mock<ILocationRepository> _locationRepoMock;
    private readonly Mock<IMarketRepository> _marketRepoMock;
    private readonly SearchVehiclesQueryHandler _handler;

    private const int BogotaLocationId = 1;
    private const int MedellinLocationId = 2;

    public SearchVehiclesQueryHandlerTests()
    {
        _vehicleRepoMock = new Mock<IVehicleRepository>();
        _locationRepoMock = new Mock<ILocationRepository>();
        _marketRepoMock = new Mock<IMarketRepository>();

        _handler = new SearchVehiclesQueryHandler(
            _vehicleRepoMock.Object,
            _locationRepoMock.Object,
            _marketRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsVehicles_WhenAvailableInLocation()
    {
        // Arrange
        var location = new Location("Bogotá", "Colombia");
        var vehicle = new Vehicle("Toyota", "Corolla", 2023, "ABC-123",
            VehicleStatus.Available, "sedan", "Colombia", BogotaLocationId);

        _locationRepoMock.Setup(x => x.GetByIdAsync(BogotaLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _marketRepoMock.Setup(x => x.IsMarketActiveAsync("Colombia", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vehicleRepoMock.Setup(x => x.SearchAvailableVehiclesAsync(
                BogotaLocationId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle> { vehicle });

        var query = new SearchVehiclesQuery(
            BogotaLocationId, MedellinLocationId,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Toyota", result[0].Brand);
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenMarketIsInactive()
    {
        // Arrange
        var location = new Location("Bogotá", "Mexico");

        _locationRepoMock.Setup(x => x.GetByIdAsync(BogotaLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _marketRepoMock.Setup(x => x.IsMarketActiveAsync("Mexico", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new SearchVehiclesQuery(
            BogotaLocationId, MedellinLocationId,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenPickupDateAfterDropoff()
    {
        var query = new SearchVehiclesQuery(
            BogotaLocationId, MedellinLocationId,
            DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(1));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenLocationNotFound()
    {
        _locationRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Location?)null);

        var query = new SearchVehiclesQuery(
            999, MedellinLocationId,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_FiltersOutVehiclesFromDifferentCountry()
    {
        // Arrange - Vehículo de otro país en localidad de Colombia
        var location = new Location("Bogotá", "Colombia");
        var vehicleOtherCountry = new Vehicle("Ford", "Mustang", 2024, "MNO-345",
            VehicleStatus.Available, "sport", "Mexico", BogotaLocationId);

        _locationRepoMock.Setup(x => x.GetByIdAsync(BogotaLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _marketRepoMock.Setup(x => x.IsMarketActiveAsync("Colombia", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vehicleRepoMock.Setup(x => x.SearchAvailableVehiclesAsync(
                BogotaLocationId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle> { vehicleOtherCountry });

        var query = new SearchVehiclesQuery(
            BogotaLocationId, MedellinLocationId,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - No debe retornar el vehículo de otro país en mercado Colombia
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_FiltersVehiclesByType_WhenVehicleTypeProvided()
    {
        // Arrange
        var location = new Location("Bogotá", "Colombia");
        var vehicleSuv = new Vehicle("Chevrolet", "Tracker", 2024, "DEF-456",
            VehicleStatus.Available, "suv", "Colombia", BogotaLocationId);

        _locationRepoMock.Setup(x => x.GetByIdAsync(BogotaLocationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(location);

        _marketRepoMock.Setup(x => x.IsMarketActiveAsync("Colombia", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vehicleRepoMock.Setup(x => x.SearchAvailableVehiclesAsync(
                BogotaLocationId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), "suv", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Vehicle> { vehicleSuv });

        var query = new SearchVehiclesQuery(
            BogotaLocationId, MedellinLocationId,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3),
            "suv");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("suv", result[0].VehicleType);
    }
}

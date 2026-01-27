using System.Net;
using System.Text.Json;
using OutletRentalCars.Application.Vehicles.DTOs;
using OutletRentalCars.Infrastructure.Seed;

namespace OutletRentalCars.Tests.Integration;

public class VehicleSearchEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VehicleSearchEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_ReturnsOk_WithAvailableVehicles()
    {
        var pickupDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");
        var dropoffDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");

        var url = $"/api/vehicles/search?pickupLocationId={DbSeeder.LocationBogotaId}" +
                  $"&dropoffLocationId={DbSeeder.LocationMedellinId}" +
                  $"&pickupDate={pickupDate}&dropoffDate={dropoffDate}";

        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleSearchResultDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(vehicles);
        Assert.True(vehicles.Count > 0);
    }

    [Fact]
    public async Task Search_ReturnsBadRequest_WhenPickupDateAfterDropoff()
    {
        var pickupDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");
        var dropoffDate = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd");

        var url = $"/api/vehicles/search?pickupLocationId={DbSeeder.LocationBogotaId}" +
                  $"&dropoffLocationId={DbSeeder.LocationMedellinId}" +
                  $"&pickupDate={pickupDate}&dropoffDate={dropoffDate}";

        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_ReturnsBadRequest_WhenLocationDoesNotExist()
    {
        var pickupDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");
        var dropoffDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");
        var fakeLocationId = 999;

        var url = $"/api/vehicles/search?pickupLocationId={fakeLocationId}" +
                  $"&dropoffLocationId={DbSeeder.LocationMedellinId}" +
                  $"&pickupDate={pickupDate}&dropoffDate={dropoffDate}";

        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_ExcludesVehiclesWithOverlappingReservation()
    {
        var pickupDate = "2026-03-02";
        var dropoffDate = "2026-03-04";

        var url = $"/api/vehicles/search?pickupLocationId={DbSeeder.LocationBogotaId}" +
                  $"&dropoffLocationId={DbSeeder.LocationMedellinId}" +
                  $"&pickupDate={pickupDate}&dropoffDate={dropoffDate}";

        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleSearchResultDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(vehicles);
        // Vehicle1 (Toyota Corolla) NO debería aparecer, Vehicle2 (Chevrolet Tracker) sí
        Assert.DoesNotContain(vehicles, v => v.Id == DbSeeder.Vehicle1Id);
    }

    [Fact]
    public async Task Search_FiltersByVehicleType()
    {
        var pickupDate = DateTime.UtcNow.AddDays(10).ToString("yyyy-MM-dd");
        var dropoffDate = DateTime.UtcNow.AddDays(15).ToString("yyyy-MM-dd");

        var url = $"/api/vehicles/search?pickupLocationId={DbSeeder.LocationBogotaId}" +
                  $"&dropoffLocationId={DbSeeder.LocationMedellinId}" +
                  $"&pickupDate={pickupDate}&dropoffDate={dropoffDate}" +
                  $"&vehicleType=suv";

        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var vehicles = JsonSerializer.Deserialize<List<VehicleSearchResultDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(vehicles);
        Assert.All(vehicles, v => Assert.Equal("suv", v.VehicleType));
    }
}

namespace OutletRentalCars.Application.Vehicles.DTOs;

public class VehicleSearchResultDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

namespace OutletRentalCars.Domain.Entities;

public class Market
{
    public string Id { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

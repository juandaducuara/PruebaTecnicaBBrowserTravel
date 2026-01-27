using MediatR;
using Microsoft.AspNetCore.Mvc;
using OutletRentalCars.Application.Vehicles.Queries;

namespace OutletRentalCars.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] int pickupLocationId,
        [FromQuery] int dropoffLocationId,
        [FromQuery] string pickupDate,
        [FromQuery] string dropoffDate,
        [FromQuery] string? vehicleType,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!DateTime.TryParse(pickupDate, out var pickup))
                return BadRequest(new { error = "Formato de fecha de recogida inválido. Use yyyy-MM-dd." });

            if (!DateTime.TryParse(dropoffDate, out var dropoff))
                return BadRequest(new { error = "Formato de fecha de devolución inválido. Use yyyy-MM-dd." });

            var query = new SearchVehiclesQuery(pickupLocationId, dropoffLocationId,
                DateTime.SpecifyKind(pickup, DateTimeKind.Utc),
                DateTime.SpecifyKind(dropoff, DateTimeKind.Utc),
                vehicleType);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

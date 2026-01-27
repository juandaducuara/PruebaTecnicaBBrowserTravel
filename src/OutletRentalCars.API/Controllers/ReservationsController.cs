using MediatR;
using Microsoft.AspNetCore.Mvc;
using OutletRentalCars.Application.Reservations.Commands;

namespace OutletRentalCars.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var reservationId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Create), new { id = reservationId }, new { id = reservationId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}

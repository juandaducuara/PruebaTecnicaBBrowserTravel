using MediatR;
using Microsoft.Extensions.Logging;
using OutletRentalCars.Domain.Events;

namespace OutletRentalCars.Application.Reservations.EventHandlers;

public class VehicleReservedEventHandler : INotificationHandler<VehicleReservedEvent>
{
    private readonly ILogger<VehicleReservedEventHandler> _logger;

    public VehicleReservedEventHandler(ILogger<VehicleReservedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(VehicleReservedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Evento de dominio: Vehículo {VehicleId} reservado. Reserva: {ReservationId}, Cliente: {Customer}, Desde: {From} Hasta: {To}",
            notification.VehicleId,
            notification.ReservationId,
            notification.CustomerName,
            notification.PickupDate,
            notification.DropoffDate);

        return Task.CompletedTask;
    }
}

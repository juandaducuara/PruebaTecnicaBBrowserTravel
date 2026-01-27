using MediatR;
using OutletRentalCars.Application.Interfaces;
using OutletRentalCars.Domain.Entities;

namespace OutletRentalCars.Application.Reservations.Commands;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, int>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMediator _mediator;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IMediator mediator)
    {
        _reservationRepository = reservationRepository;
        _mediator = mediator;
    }

    public async Task<int> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        if (request.PickupDate >= request.DropoffDate)
            throw new ArgumentException("La fecha de recogida debe ser anterior a la fecha de devolución.");

        // Verificar que no haya reservas cruzadas
        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(
            request.VehicleId, request.PickupDate, request.DropoffDate, cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("El vehículo ya tiene una reserva activa en ese rango de fechas.");

        var reservation = Reservation.Create(
            request.VehicleId,
            request.PickupLocationId,
            request.DropoffLocationId,
            request.PickupDate,
            request.DropoffDate,
            request.CustomerName);

        await _reservationRepository.AddAsync(reservation, cancellationToken);

        // Publicar eventos de dominio
        foreach (var domainEvent in reservation.DomainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        reservation.ClearDomainEvents();

        return reservation.Id;
    }
}

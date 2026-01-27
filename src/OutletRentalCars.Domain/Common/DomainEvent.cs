using MediatR;

namespace OutletRentalCars.Domain.Common;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

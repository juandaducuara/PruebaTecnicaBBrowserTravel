namespace OutletRentalCars.Domain.Common;

public abstract class Entity
{
    private readonly List<DomainEvent> _domainEvents = new();

    public int Id { get; protected set; }

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

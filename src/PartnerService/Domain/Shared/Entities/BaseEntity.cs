
using PartnerService.Domain.Shared.Events;

namespace PartnerService.Domain.Shared.Entities;

public abstract class BaseEntity : IEntity<Guid>
{
    public Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    protected BaseEntity(Guid id)
    {
        if(id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
            
        Id = id;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
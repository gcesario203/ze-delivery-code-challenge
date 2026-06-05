
namespace PartnerService.Domain.Shared.Events;

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTime OccurredAt { get; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
    }
}


using PartnerService.Domain.Shared.Events;

namespace PartnerService.Application.Shared.Contracts;

public interface IDomainEventDispatcher
{
    Task Dispatch(IEnumerable<IDomainEvent> events);
}
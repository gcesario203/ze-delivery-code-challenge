
using PartnerService.Domain.Shared.Events;

namespace PartnerService.Application.Shared.Contracts;

public interface IEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event);
}
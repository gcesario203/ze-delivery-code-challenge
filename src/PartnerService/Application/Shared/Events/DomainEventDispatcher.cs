
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Shared.Events;

namespace PartnerService.Application.Shared.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task Dispatch(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("Handle");
                if (handleMethod != null)
                {
                    await (Task)handleMethod.Invoke(handler, new object[] { @event })!;
                }
            }
        }
    }
}
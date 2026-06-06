
using PartnerService.Application.Shared.Contracts;
using Wolverine;

namespace PartnerService.Infra.Shared.EventBus;

public sealed class WolverineHandlerDispatcher : IHandlerDispatcher
{
    private readonly IMessageBus _bus;

    public WolverineHandlerDispatcher(IMessageBus bus)
    {
        _bus = bus;
    }

    public Task DispatchAsync(object command)
    {
        return _bus.InvokeAsync(command);
    }

    public Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request)
    {
        return _bus.InvokeAsync<TResponse>(request);
    }
}
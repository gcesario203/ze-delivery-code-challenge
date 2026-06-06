
namespace PartnerService.Application.Shared.Contracts;

public interface IHandlerDispatcher
{
    Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request);

    Task DispatchAsync(object command);
}
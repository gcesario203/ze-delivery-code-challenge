
namespace PartnerService.Application.Shared.Contracts;

public interface IOutboxPublisher
{
    Task EnqueueAsync<TEvent>(TEvent @event);
}

namespace PartnerService.Application.Shared.Contracts;

public interface IOutboxProcessor
{
    Task ProcessPendingAsync(CancellationToken cancellationToken = default);
}

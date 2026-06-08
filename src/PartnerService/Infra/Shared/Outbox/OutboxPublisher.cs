

using PartnerService.Application.Shared.Contracts;
using PartnerService.Infra.Shared.Outbox.Models;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Infra.Shared.Outbox;

public sealed class OutboxPublisher : IOutboxPublisher
{
    private readonly AppDbContext _dbContext;

    public OutboxPublisher(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnqueueAsync<TEvent>(TEvent @event)
    {
        var outboxMessage = new OutboxDBModel
        {
            Id = Guid.NewGuid(),
            EventType = typeof(TEvent).FullName,
            Payload = System.Text.Json.JsonSerializer.Serialize(@event),
            Status = Enums.OutboxMessageStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };

        _dbContext.OutboxMessages.Add(outboxMessage);
        await _dbContext.SaveChangesAsync();
    }
}
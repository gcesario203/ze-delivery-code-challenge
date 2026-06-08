
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Infra.Shared.Outbox.Enums;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Infra.Shared.Outbox;

public sealed class OutboxProcessor : IOutboxProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly IGeolocalizationClient _geolocalizationClient;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        AppDbContext dbContext,
        IGeolocalizationClient geolocalizationClient,
        ILogger<OutboxProcessor> logger)
    {
        _dbContext = dbContext;
        _geolocalizationClient = geolocalizationClient;
        _logger = logger;
    }

    public async Task ProcessPendingAsync(CancellationToken cancellationToken = default)
    {
        var pendingMessages = await _dbContext.OutboxMessages
            .Where(message => message.Status == OutboxMessageStatus.Pending)
            .OrderBy(message => message.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var message in pendingMessages)
        {
            message.Status = OutboxMessageStatus.Processing;
            await _dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                await DispatchAsync(message);
                message.Status = OutboxMessageStatus.Processed;
                message.ProcessedAt = DateTime.UtcNow;
                message.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                message.Status = OutboxMessageStatus.Failed;
                message.RetryCount++;
                message.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task DispatchAsync(Outbox.Models.OutboxDBModel message)
    {
        if (message.EventType == typeof(PartnerCreatedIntegrationEvent).FullName)
        {
            var integrationEvent = JsonSerializer.Deserialize<PartnerCreatedIntegrationEvent>(message.Payload)
                ?? throw new InvalidOperationException("Outbox payload could not be deserialized.");

            await _geolocalizationClient.CreatePartnerGeolocalizationAsync(integrationEvent);
            return;
        }

        throw new NotSupportedException($"Outbox event type '{message.EventType}' is not supported.");
    }
}

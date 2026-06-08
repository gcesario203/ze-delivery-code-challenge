

using PartnerService.Infra.Shared.Outbox.Enums;

namespace PartnerService.Infra.Shared.Outbox.Models;

public sealed class OutboxDBModel
{
    public Guid Id { get; set; }

    public string EventType { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public string Payload { get; set; }

    public DateTime CreatedAt { get; set; }

    public int RetryCount { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public string ErrorMessage { get; set; }
}
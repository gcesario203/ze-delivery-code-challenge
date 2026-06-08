
namespace PartnerService.Infra.Shared.Outbox.Enums;

public enum OutboxMessageStatus
{
    Pending = 0,

    Processing,
    Processed,
    Failed
}
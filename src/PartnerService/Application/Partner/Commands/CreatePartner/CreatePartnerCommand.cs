

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public sealed class CreatePartnerCommand
{
    public string TradingName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string Document { get; set; } = null!;
}
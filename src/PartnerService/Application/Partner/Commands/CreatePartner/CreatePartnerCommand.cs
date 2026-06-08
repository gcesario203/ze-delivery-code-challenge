

using PartnerService.Application.Shared.DataTransferObjects;

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public sealed class CreatePartnerCommand
{
    public string TradingName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string Document { get; set; } = null!;
    public CoverageAreaDTO CoverageArea { get; set; } = null!;
    public AddressDTO Address { get; set; } = null!;
}
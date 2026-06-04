
using PartnerService.Domain.Partner.Entities;

namespace PartnerService.Application.Partner.Queries.GetById;

public static class PartnerMapper
{
    public static PartnerViewModel ToViewModel(this PartnerEntity partner)
    {
        return new PartnerViewModel
        {
            Id = partner.Id.ToString(),
            TradingName = partner.TradingName,
            OwnerName = partner.OwnerName,
            Document = partner.Document.Value
        };
    }
}
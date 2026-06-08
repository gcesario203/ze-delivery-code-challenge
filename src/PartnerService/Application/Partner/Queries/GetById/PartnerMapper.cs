
using PartnerService.Application.GeoLocalization.DataTransferObjects;
using PartnerService.Domain.Partner.Entities;

namespace PartnerService.Application.Partner.Queries.GetById;

public static class PartnerMapper
{
    public static PartnerViewModel ToViewModel(this PartnerEntity partner, PartnerGeolocalizationDTO geolocalization)
    {
        var partnerViewModel = new PartnerViewModel
        {
            Id = partner.Id.ToString(),
            TradingName = partner.TradingName,
            OwnerName = partner.OwnerName,
            Document = partner.Document.Value,
        };

        if(geolocalization != null)
        {
            partnerViewModel.Address = geolocalization.Address;
            partnerViewModel.CoverageArea = geolocalization.CoverageArea;
        }

        return partnerViewModel;
    }
}

using PartnerService.Application.Shared.DataTransferObjects;

namespace PartnerService.Application.Partner.Queries.GetById
{
    public class PartnerViewModel
    {
        public string Id { get; set; }

        public string TradingName { get; set; }

        public string OwnerName { get; set; }

        public string Document { get; set; }

        public AddressDTO Address { get; set; }

        public CoverageAreaDTO CoverageArea { get; set; }
    }
}
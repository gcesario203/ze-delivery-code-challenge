

using PartnerService.Domain.Partner.Entities;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public static class CreatePartnerMapper
{
    public static PartnerEntity ToDomainEntity(this CreatePartnerCommand command, AddressVO address, CoverageAreaVO coverageArea)
    {
        return PartnerEntity.CreatePartner(
            command.TradingName,
            command.OwnerName,
            new CnpjVO(command.Document),
            address,
            coverageArea
        );
    }
}


using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Application.Shared.Mappers;

public static class AddressMapper
{
    public static AddressDTO ToDTO(this AddressVO address)
        => new AddressDTO
        {
            Cordinates = address.Cordinates.ToDTO()
        };

    public static AddressVO ToVO(this AddressDTO address)
        => new AddressVO(address.Cordinates.ToVO());
}
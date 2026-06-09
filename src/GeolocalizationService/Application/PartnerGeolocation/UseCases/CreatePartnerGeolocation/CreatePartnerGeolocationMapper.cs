
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public static class CreatePartnerGeolocationMapper
{
    public static PartnerGeolocationEntity ToDomainEntity(this CreatePartnerGeolocationInput input)
    {
        return PartnerGeolocationEntity.Create(input.Id, input.Address.ToVO(), input.CoverageArea.ToVO());
    }

    public static PartnerGeolocationViewModel ToViewModel(this PartnerGeolocationEntity partnerGeolocation)
    {
        return new PartnerGeolocationViewModel
        {
            Id = partnerGeolocation.Id,
            Address = partnerGeolocation.Address.ToDTO(),
            CoverageArea = partnerGeolocation.CoverageArea.ToDTO()
        };
    }

    public static AddressDTO ToDTO(this AddressVO address)
    {
        return new AddressDTO
        {
            Cordinates = new CordinateDTO
            {
                Latitude = address.Cordinates.Latitude,
                Longitude = address.Cordinates.Longitude
            }
        };
    }

    public static CoverageAreaDTO ToDTO(this CoverageAreaVO coverageArea)
    {
        return new CoverageAreaDTO
        {
            Cordinates = coverageArea.Cordinates.Select(c => c.Select(coord => coord.Select(cord => new CordinateDTO
            {
                Latitude = cord.Latitude,
                Longitude = cord.Longitude
            }).ToList()).ToList()).ToList()
        };
    }

    public static AddressVO ToVO(this AddressDTO address)
    {
        return new AddressVO(new CordinateVO(address.Cordinates.Latitude, address.Cordinates.Longitude));
    }

    public static CoverageAreaVO ToVO(this CoverageAreaDTO coverageArea)
    {
        return new CoverageAreaVO(coverageArea.Cordinates.Select(c => c.Select(coord => coord.Select(cord => new CordinateVO(cord.Latitude, cord.Longitude)).ToList()).ToList()).ToList());
    }
}
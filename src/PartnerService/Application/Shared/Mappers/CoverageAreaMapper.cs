
using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Application.Shared.Mappers;

public static class CoverageAreaMapper
{
    public static CoverageAreaDTO ToDTO(this CoverageAreaVO coverageArea)
        => new CoverageAreaDTO
        {
            Cordinates = coverageArea.Cordinates.ToDTO()
        };

    public static CoverageAreaVO ToVO(this CoverageAreaDTO coverageArea)
        => new CoverageAreaVO(coverageArea.Cordinates.Select(c1 => c1.Select(c2 => c2.Select(c3 => c3.ToVO())
                                                                     .ToList())
                                                                     .ToList())
                                                                     .ToList());
}
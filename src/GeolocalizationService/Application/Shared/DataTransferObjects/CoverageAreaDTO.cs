
namespace GeolocalizationService.Application.Shared.DataTransferObjects;

public class CoverageAreaDTO
{
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> Cordinates { get; set; }
}
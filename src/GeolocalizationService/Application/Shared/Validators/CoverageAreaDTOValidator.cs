

namespace GeolocalizationService.Application.Shared.Validators;

using FluentValidation;
using GeolocalizationService.Application.Shared.DataTransferObjects;

public class CoverageAreaDTOValidator : AbstractValidator<CoverageAreaDTO>
{
    public CoverageAreaDTOValidator()
    {
        RuleFor(c => c.Cordinates)
            .NotEmpty()
            .WithMessage("Coverage area coordinates cannot be empty.")
            .Must(HaveAtLeastOnePolygon)
            .WithMessage("Coverage area must have at least one polygon.")
            .Must(AllRingsHaveMinimumPoints)
            .WithMessage("Each ring must have at least 5 coordinate points (4 distinct vertices plus the closing point).")
            .Must(AllRingsHaveMinimumDistinctVertices)
            .WithMessage("Each ring must have at least 4 distinct coordinate points.")
            .Must(AllRingsAreClosed)
            .WithMessage("Each ring must be closed (first coordinate must equal last coordinate).")
            .Must(AllCoordinatesAreValid)
            .WithMessage("All coordinates must have valid latitude (-90 to 90) and longitude (-180 to 180).");
    }

    private static bool HaveAtLeastOnePolygon(
        IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> coordinates) =>
        coordinates.Count > 0;

    private static bool AllRingsHaveMinimumPoints(
        IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> coordinates) =>
        coordinates.All(polygon => polygon.All(ring => ring.Count >= 5));

    private static bool AllRingsHaveMinimumDistinctVertices(
        IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> coordinates) =>
        coordinates.All(polygon => polygon.All(ring =>
            ring.Select(coordinate => (coordinate.Latitude, coordinate.Longitude)).Distinct().Count() >= 4));

    private static bool AllRingsAreClosed(
        IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> coordinates) =>
        coordinates.All(polygon => polygon.All(ring =>
            ring.Count >= 5 &&
            ring.First().Latitude == ring.Last().Latitude &&
            ring.First().Longitude == ring.Last().Longitude));

    private static bool AllCoordinatesAreValid(
        IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> coordinates) =>
        coordinates.All(polygon => polygon.All(ring => ring.All(coord =>
            coord.Latitude is >= -90 and <= 90 &&
            coord.Longitude is >= -180 and <= 180)));
}

namespace GeolocalizationService.Application.Shared.ValueObjects;

public class ValidationFailureVO
{
    public string PropertyName { get; }
    public string ErrorMessage { get; }

    public ValidationFailureVO(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}
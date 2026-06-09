
using GeolocalizationService.Application.Shared.ValueObjects;

namespace GeolocalizationService.Application.Shared.Exceptions;

    public class UseCaseValidationException : Exception
    {
        public IEnumerable<ValidationFailureVO> Errors { get; }

        public UseCaseValidationException(string message, IEnumerable<ValidationFailureVO> errors) : base(message)
        {
            Errors = errors;
        }
    }
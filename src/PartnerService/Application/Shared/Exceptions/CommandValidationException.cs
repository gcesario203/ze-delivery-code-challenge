

using PartnerService.Application.Shared.ValueObjects;

namespace PartnerService.Application.Shared.Exceptions;

public class CommandValidationException : Exception
{
    public IEnumerable<ValidationFailureVO> Errors { get; }

    public CommandValidationException(string message, IEnumerable<ValidationFailureVO> errors) : base(message)
    {
        Errors = errors;
    }
}
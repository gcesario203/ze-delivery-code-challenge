

using PartnerService.Application.Shared.ValueObjects;

namespace PartnerService.Application.Shared.Exceptions;

public class CommandValidationException : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; }

    public CommandValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message)
    {
        Errors = errors;
    }
}
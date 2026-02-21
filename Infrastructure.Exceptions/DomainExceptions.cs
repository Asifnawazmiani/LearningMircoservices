namespace Infrastructure.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }

    public class NotFoundException : DomainException
    {
        public NotFoundException(string entity, object id)
            : base($"{entity} with id {id} was not found") { }
    }

    public class DuplicateException : DomainException
    {
        public DuplicateException(string message) : base(message) { }
    }

    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message = "Unauthorized")
            : base(message) { }
    }

    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message = "Forbidden")
            : base(message) { }
    }

    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }

    public class ValidationException : DomainException
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }

        public ValidationException(string field, string message)
            : base("Validation failed")
        {
            Errors = new List<ValidationError> { new(field, message) };
        }
    }

    public record ValidationError(string? Field, string Message);
}

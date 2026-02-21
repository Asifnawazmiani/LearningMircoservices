namespace Shared.Common.Results;

public record Error(string Code, string Message)
{
    public static readonly Error None
        = new(string.Empty, string.Empty);

    // Common errors
    public static readonly Error NullValue
        = new("NULL_VALUE", "Value cannot be null");

    // Factory methods
    public static Error NotFound(string entity, object id)
        => new("NOT_FOUND", $"{entity} with id {id} was not found");

    public static Error Duplicate(string message)
        => new("DUPLICATE", message);

    public static Error Validation(string message)
        => new("VALIDATION", message);

    public static Error Unauthorized(string message = "Unauthorized")
        => new("UNAUTHORIZED", message);

    public static Error Forbidden(string message = "Forbidden")
        => new("FORBIDDEN", message);

    public static Error Conflict(string message)
        => new("CONFLICT", message);

    public static Error Internal(string message = "Internal server error")
        => new("INTERNAL", message);
}
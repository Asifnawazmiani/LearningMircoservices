using System;

namespace UserService.Domain.Exceptions
{
    public class UserDomainException : DomainException
    {
        public UserDomainException(string message)
            : base("USER_ERROR", message) { }
    }

    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(Guid id)
            : base("User", id) { }
    }

    public class UserSuspendedException : DomainException
    {
        public UserSuspendedException(Guid userId)
            : base("USER_SUSPENDED", $"User {userId} is suspended") { }
    }

    public class AddressNotFoundException : NotFoundException
    {
        public AddressNotFoundException(Guid id)
            : base("Address", id) { }
    }

    public class DuplicateEmailException : DomainException
    {
        public DuplicateEmailException(string email)
            : base("DUPLICATE_EMAIL", $"Email {email} is already registered") { }
    }
}

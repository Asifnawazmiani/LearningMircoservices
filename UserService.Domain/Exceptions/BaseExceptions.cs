using System;

namespace UserService.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Code { get; }

        protected DomainException(string code, string message)
            : base(message)
        {
            Code = code;
        }
    }

    public abstract class NotFoundException : DomainException
    {
        protected NotFoundException(string entityName, Guid id)
            : base($"{entityName.ToUpper()}_NOT_FOUND", $"{entityName} with id '{id}' was not found") { }
    }
}

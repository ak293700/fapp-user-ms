namespace Domain.Common.Exceptions;

public class DomainException : CustomException
{
    public DomainException(string message) : base(message)
    {
    }

    public class NotFoundDomainException : DomainException
    {
        public NotFoundDomainException(string message) : base(message)
        {}

        public static NotFoundDomainException Instance => new("Resources not found");
    }
    
    public class AlreadyExistDomainException : DomainException
    {
        public AlreadyExistDomainException(string message) : base(message)
        {}

        public static AlreadyExistDomainException Instance => new("This resource already exist");
    }
}
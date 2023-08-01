using Domain.Common.Exceptions;

namespace Application.Common.Exceptions;

public class ApplicationException : CustomException
{
    public ApplicationException(string message) : base(message)
    {}
}
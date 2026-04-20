namespace Identity.Domain.Exceptions;

public sealed class InvalidEmailException : Exception
{
    
    public InvalidEmailException()
        : base("Invalid email address has been specified.") { }
    public InvalidEmailException(string email)
        : base($"'{email}' is not a valid email address.") { }
    
    public InvalidEmailException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
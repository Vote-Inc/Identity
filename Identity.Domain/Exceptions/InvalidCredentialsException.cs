public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException() 
        : base("Invalid credentials.") { }
}
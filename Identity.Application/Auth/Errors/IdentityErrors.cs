namespace Identity.Application.Auth.Errors;

public static class IdentityErrors
{
    public static readonly Error InvalidCredentials = 
        new("identity.credentials.invalid", "Invalid email or password.");
    
    public static readonly Error UserNotFound = 
        new("identity.user.not_found", "No account was found for the provided email.");
}

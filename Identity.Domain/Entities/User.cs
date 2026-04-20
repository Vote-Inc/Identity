namespace Identity.Domain.Entities;

public sealed class User : Entity, IAggregateRoot
{
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(Email email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash
        };
    }

    public static User Reconstitute(Guid id, Email email, DateTime? lastLoginAt)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = string.Empty,
            LastLoginAt = lastLoginAt
        };
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        AddDomainEvent(new UserLoggedInEvent(Id));
    }

    public void RecordFailedLogin()
    {
        AddDomainEvent(new UserLogInFailedEvent(Email.Value));
    }
}

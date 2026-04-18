namespace Identity.Domain.Events;

public sealed record UserLoggedInEvent : INotification
{
    public Guid UserId { get; private set; }

    public UserLoggedInEvent(Guid userId)
    {
        UserId = userId;
    }
};
namespace Identity.Domain.Events;

public class UserLoggedInEvent
    : INotification
{
    public User User { get; private set; }
    public DateTime OccurredAt { get; private set;}


    public UserLoggedInEvent(User user, DateTime occuredAt)
    {
        User = user;
        OccurredAt = occuredAt;
    }
}
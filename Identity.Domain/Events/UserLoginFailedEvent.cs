namespace Identity.Domain.Events;

public class UserLogInFailedEvent
    : INotification
{
    public string Email { get; private set; }
    public DateTime OccurredAt { get; private set; }


    public UserLogInFailedEvent(string email, DateTime occuredAt)
    {
        Email = email;
        OccurredAt = occuredAt;
    }
}
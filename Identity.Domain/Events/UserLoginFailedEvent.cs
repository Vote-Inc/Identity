namespace Identity.Domain.Events;

public class UserLogInFailedEvent
    : INotification
{
    public string Email { get; private set; }

    public UserLogInFailedEvent(string email)
    {
        Email = email;
    }
}
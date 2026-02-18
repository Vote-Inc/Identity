using System.ComponentModel.DataAnnotations;

namespace Identity.Domain.AggregatesModel.UserAggregate;

public class User : Entity, IAggregateRoot
{
    [Required]
    public Email Email { get; private set; }

    public User(Email email)
    {
        Email = email;
    }
}

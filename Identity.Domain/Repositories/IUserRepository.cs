namespace Identity.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);
}

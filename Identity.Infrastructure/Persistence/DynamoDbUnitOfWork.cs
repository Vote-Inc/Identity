namespace Identity.Infrastructure.Persistence;

internal sealed class DynamoDbUnitOfWork(IAmazonDynamoDB dynamoDb, string tableName) : IUnitOfWork
{
    private readonly List<User> _tracked = [];

    internal void Track(User user) => _tracked.Add(user);

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var user in _tracked)
        {
            await dynamoDb.PutItemAsync(tableName, ToItem(user), cancellationToken);
        }

        _tracked.Clear();
        return true;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var count = _tracked.Count;
        await SaveEntitiesAsync(cancellationToken);
        return count;
    }

    private static Dictionary<string, AttributeValue> ToItem(User user)
    {
        var item = new Dictionary<string, AttributeValue>
        {
            ["pk"]     = new() { S = user.Email.Value },
            ["userId"] = new() { S = user.Id.ToString() },
        };

        if (user.LastLoginAt.HasValue)
            item["lastLoginAt"] = new() { S = user.LastLoginAt.Value.ToString("O") };

        return item;
    }

    public void Dispose() { }
}

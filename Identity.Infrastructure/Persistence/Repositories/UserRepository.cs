namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly string _tableName;
    private readonly DynamoDbUnitOfWork _unitOfWork;

    public UserRepository(IAmazonDynamoDB dynamoDb, IOptions<DynamoDbSettings> settings)
    {
        _dynamoDb = dynamoDb;
        _tableName = settings.Value.TableName;
        _unitOfWork = new DynamoDbUnitOfWork(dynamoDb, _tableName);
    }

    public IUnitOfWork UnitOfWork => _unitOfWork;

    public async Task<User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var response = await _dynamoDb.GetItemAsync(
            _tableName,
            new Dictionary<string, AttributeValue>
            {
                ["pk"] = new() { S = email.Value }
            },
            cancellationToken);

        return response.IsItemSet ? MapToUser(response.Item) : null;
    }

    public void Add(User user) => _unitOfWork.Track(user);

    public void Update(User user) => _unitOfWork.Track(user);

    private static User? MapToUser(Dictionary<string, AttributeValue> item)
    {
        try
        {
            var email = Email.Create(item["pk"].S);

            var userId = Guid.Parse(item["userId"].S);

            DateTime? lastLoginAt = item.TryGetValue("lastLoginAt", out var la)
                ? DateTime.Parse(la.S, null, System.Globalization.DateTimeStyles.RoundtripKind)
                : null;

            return User.Reconstitute(userId, email, lastLoginAt);
        }
        catch (InvalidEmailException)
        {
            return null;
        }
    }
}

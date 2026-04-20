namespace Identity.Infrastructure.Persistence;

public sealed class DynamoDbSettings
{
    public const string SectionName = "DynamoDb";

    public string TableName { get; init; } = string.Empty;
    
    public string? ServiceUrl { get; init; }
}

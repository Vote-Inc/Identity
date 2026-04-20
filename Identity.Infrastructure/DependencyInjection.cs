namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cognitoSettings = configuration.GetSection(CognitoSettings.SectionName).Get<CognitoSettings>()!;
        var dynamoDbSettings = configuration.GetSection(DynamoDbSettings.SectionName).Get<DynamoDbSettings>()!;
        var region = RegionEndpoint.GetBySystemName(cognitoSettings.Region);

        services.Configure<CognitoSettings>(configuration.GetSection(CognitoSettings.SectionName));
        services.Configure<DynamoDbSettings>(configuration.GetSection(DynamoDbSettings.SectionName));

        services.AddSingleton<IAmazonCognitoIdentityProvider>(_ =>
            new AmazonCognitoIdentityProviderClient(region));

        services.AddSingleton<IAmazonDynamoDB>(_ =>
        {
            if (!string.IsNullOrEmpty(dynamoDbSettings.ServiceUrl))
            {
                return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
                {
                    ServiceURL = dynamoDbSettings.ServiceUrl
                });
            }

            return new AmazonDynamoDBClient(region);
        });

        services.AddScoped<ICognitoService, CognitoService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;
using Testcontainers.MongoDb;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();

    public async Task InitializeAsync() => await _mongoContainer.StartAsync();

    public new async Task DisposeAsync() => await _mongoContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var connectionString = _mongoContainer.GetConnectionString();
            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase("IntegrationTestDb");

            services.AddSingleton(database);
        });
    }
}
using InventorySystem_API.Company.Models;
using InventorySystem_API.User.Model;
using InventorySystem_Shared.AddressClass;
using InventorySystem_Shared.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Bson;
using MongoDB.Driver;
using Testcontainers.MongoDb;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder().Build();

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        var _connection = _mongoContainer.GetConnectionString();
        var client = new MongoClient(_connection);
        var database = client.GetDatabase("IntegrationTestDb");

        #region CompanySeedData
        var address = new Address
        {
            Country = "Test Country",
            State = "Test State",
            District = "Test District",
            City = "Test City",
            Street = "Test Street",
            HouseNumber = "123",
            Longitude = 45.0,
            Latitude = 90.0
        };

        var companies = database.GetCollection<CompanyModel>("Companies");
        var testCompany = new CompanyModel
        {
            Name = "Test Corp",
            Phone = "+380123456789",
            Description = "A company for integration testing",
            Address = address

        };
        await companies.InsertOneAsync(testCompany);

        #endregion

        #region UserSeedData
        var users = database.GetCollection<UserModel>("Users");


        var password = "Password_555";

        var testUsers = new List<UserModel>
        {
            new UserModel
            {
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                UserRole = UserRole.admin,
                CompanyId = testCompany.Id,
                PhotoURI = "/Images/User/default.png",
                Name = "Admin User"
            },
            new UserModel
            {
                Email = "manager@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                UserRole = UserRole.manager,
                CompanyId = testCompany.Id,
                PhotoURI = "/Images/User/default.png",
                Name = "Manager User"
            }
        };
        await users.InsertManyAsync(testUsers);

        #endregion
    }

    public new async Task DisposeAsync() => await _mongoContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoDatabase));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            var mongoClient = new MongoClient(_mongoContainer.GetConnectionString());
            var database = mongoClient.GetDatabase("IntegrationTestDb");

            services.AddSingleton<IMongoDatabase>(database);
        });
    }
}
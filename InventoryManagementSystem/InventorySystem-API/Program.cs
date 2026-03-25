using FluentValidation;
using FluentValidation.AspNetCore;
using InventorySystem_API.Company.Repository;
using InventorySystem_API.Company.Service;
using InventorySystem_API.Company.Validation;
using InventorySystem_API.External_API.Adress;
using InventorySystem_API.Inventory.Repository;
using InventorySystem_API.Inventory.Service;
using InventorySystem_API.Inventory.Validator;
using InventorySystem_API.Loging.Repository;
using InventorySystem_API.Service.Image;
using InventorySystem_API.User.Model;
using InventorySystem_API.User.Repositories;
using InventorySystem_API.User.Services;
using InventorySystem_API.User.Validator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region MongoDB
var stringConnection = builder.Configuration["MongoDB:ConnectionString"];
var databaseName = builder.Configuration["MongoDB:DataBaseName"];

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(stringConnection)
);
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});

#endregion

#region A&A
builder.Services.Configure<JWTSettingOptions>(
    builder.Configuration.GetSection("JWTSettings"));

var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettingOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!))
    };
});

builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<TokenGenerator>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserModelValidator>();

#endregion

#region User
builder.Services.AddScoped<IUserService, UserService>();
#endregion

#region Company
builder.Services.AddValidatorsFromAssemblyContaining<CompanyValidator>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
#endregion

#region Inventory
builder.Services.AddValidatorsFromAssemblyContaining<InventoryValidator>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
#endregion

#region Loging
builder.Services.AddScoped<ILogRepository, LogRepository>();
#endregion

#region Services
builder.Services.Configure<GeopifyAPIKeys>(
    builder.Configuration.GetSection("Geopify"));

builder.Services.AddScoped<IAddressService, GeopifyAddressService>();
builder.Services.AddSingleton<IImageService, ImageService>();
#endregion



builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();

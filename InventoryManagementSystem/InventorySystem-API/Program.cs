using MongoDB.Driver;

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

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

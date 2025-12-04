using Lab2.DAL;
using Lab2.DAL.Settings;
using Lab2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LabDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
    clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    clientSettings.ConnectTimeout = TimeSpan.FromSeconds(5);
    clientSettings.SocketTimeout = TimeSpan.FromSeconds(5);
    return new MongoClient(clientSettings);
});

builder.Services.AddSingleton<StudentService>();
builder.Services.AddSingleton<MenuService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LabDbContext>();
    DbInitializer.Initialize(context);
}

var menuService = app.Services.GetRequiredService<MenuService>();
await menuService.RunAsync();

using Microsoft.EntityFrameworkCore;
using TestManager.Application.Repositories;
using TestManager.Application.Services;
using TestManager.Infrastructure.Persistence;
using TestManager.Infrastructure.Repositories;

const string desktopClientCorsPolicy = "DesktopClient";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(desktopClientCorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
    await DemoDataSeeder.SeedAsync(dbContext);
}

app.UseCors(desktopClientCorsPolicy);
app.MapControllers();

app.Run();

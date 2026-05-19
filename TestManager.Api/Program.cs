using Microsoft.EntityFrameworkCore;
using TestManager.Application.Repositories;
using TestManager.Application.Services;
using TestManager.Infrastructure.Persistence;
using TestManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using VacationPlanner.Data;
using VacationPlanner.Implementation;
using VacationPlanner.Implementation.Services;
using VacationPlanner.Interfaces;
using VacationPlanner.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IDbHealthService, PostgresDbHealthService>();
builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

    var config = $"{options.Host}:{options.Port}";

    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//Добавление сервисов AdministrationController

builder.Services.AddScoped<IPositionRepository, EfPositionRepository>();
builder.Services.AddScoped<IVacationDurationRepository, EfVacationDurationRepository>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IVacationDurationService, VacationDurationService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Implementation.EventHandlers.Notifications;
using VacationPlanner.Implementation.Events;
using VacationPlanner.Implementation.Helpers;
using VacationPlanner.Implementation.Infrastructure;
using VacationPlanner.Implementation.Repository;
using VacationPlanner.Implementation.Services;
using VacationPlanner.Interfaces.Helpers;
using VacationPlanner.Interfaces.Infrastructure;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Options;
using VacationPlanner.Models.Requests;
using VacationPlanner.Validators;
using VacationPlanner.Models.Enums;
using VacationPlanner.Models.Requests;
using VacationPlanner.Implementation.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };
    });
var conf = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INotificationService, NotificationService>();
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
builder.Services.AddScoped<IPositionRepository, EfPositionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IVacationDurationRepository, EfVacationDurationRepository>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IVacationDurationService, VacationDurationService>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<
    IValidator<ChangePasswordRequest>,
    ChangePasswordRequestValidator>();
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddScoped<IEventHandler<VacationRequestCreatedEvent>, VacationRequestCreatedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationRequestSubmittedEvent>, VacationRequestSubmittedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationRequestApprovedByManagerEvent>, VacationRequestApprovedByManagerNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationRequestRejectedByManagerEvent>, VacationRequestRejectedByManagerNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationRequestApprovedByHrEvent>, VacationRequestApprovedByHrNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationRequestRejectedByHrEvent>, VacationRequestRejectedByHrNotificationHandler>();
builder.Services.AddScoped<IEventHandler<VacationCreatedEvent>, VacationCreatedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredNotificationHandler>();
builder.Services.AddScoped<IEventHandler<PasswordChangedEvent>, PasswordChangedNotificationHandler>();
builder.Services.AddScoped<IEventHandler<PasswordRestoreRequestedEvent>, PasswordRestoreRequestedNotificationHandler>();


//---РЕГИСТРАЦИЯ РЕПОЗИТОРИЕВ(добавить к существующим Scoped - регистрациям)-- -
builder.Services.AddScoped<IVacationRequestRepository, EfVacationRequestRepository>();
builder.Services.AddScoped<IVacationRepository, EfVacationRepository>();
builder.Services.AddScoped<IVacationApprovalRepository, EfVacationApprovalRepository>();

//---РЕГИСТРАЦИЯ СЕРВИСОВ(добавить к существующим Scoped - регистрациям)-- -
builder.Services.AddScoped<IVacationRequestService, VacationRequestService>();
builder.Services.AddScoped<IVacationService, VacationService>();
builder.Services.AddScoped<IHrService, HrService>();

//---РЕГИСТРАЦИЯ ВАЛИДАТОРОВ(добавить к существующим)-- -
builder.Services.AddScoped<IValidator<CreateVacationRequest>, CreateVacationRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateVacationRequest>, UpdateVacationRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Введите JWT токен в формате: Bearer {token}"
        });


    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


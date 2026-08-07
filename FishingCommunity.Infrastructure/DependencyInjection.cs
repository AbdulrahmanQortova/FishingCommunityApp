using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Infrastructure.BackgroundJobs;
using FishingCommunity.Infrastructure.Identity;
using FishingCommunity.Infrastructure.Messaging;
using FishingCommunity.Infrastructure.Persistence;
using FishingCommunity.Infrastructure.Persistence.Interceptors;
using FishingCommunity.Infrastructure.Services;
using FishingCommunity.Infrastructure.Services.AI;
using FishingCommunity.Infrastructure.Services.Chat;
using FishingCommunity.Infrastructure.Services.Email;
using FishingCommunity.Infrastructure.Services.FileStorage;
using FishingCommunity.Infrastructure.Services.Weather;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FishingCommunity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // --- Options binding ---
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<FeatureFlags>(configuration.GetSection(FeatureFlags.SectionName));
        services.Configure<WeatherSettings>(configuration.GetSection(WeatherSettings.SectionName));
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        // --- Interceptors ---
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        // --- DbContext ---
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        });

        #region Redis
        // Redis distributed cache — used by IDistributedCache consumers (WeatherService, and
        // anything else that needs caching going forward).
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "FishingCommunity:";
            });
        }
        else
        {
            // Fallback for local development without Redis running — an in-memory cache
            // implements the same IDistributedCache interface, so WeatherService works
            // unchanged either way.
            services.AddDistributedMemoryCache();
        } 
        #endregion
        // --- ASP.NET Core Identity ---
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;

            // We manage email confirmation manually via our own EmailVerificationToken entity,
            // rather than Identity's built-in confirmation token flow.
            options.SignIn.RequireConfirmedEmail = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();




        services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
        configuration.GetConnectionString("DefaultConnection"),
        new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 2;
        });
        // --- JWT Authentication ---
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing from configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Allow SignalR to receive the JWT via query string (needed because browsers
                // can't set Authorization headers on WebSocket connections).
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        // --- HttpContextAccessor (needed by CurrentUserService) ---
        services.AddHttpContextAccessor();

        // --- Application services / infrastructure implementations ---
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<IChatConnectionTracker, ChatConnectionTracker>();
        services.AddScoped<IChatNotifier, ChatNotifier>();
        services.AddScoped<IAiAssistantService, RuleBasedAiAssistantService>();
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        // Job classes themselves — registered as Scoped since they use IUnitOfWork internally.
        services.AddScoped<TripReminderJob>();
        services.AddScoped<CleanupExpiredTokensJob>();
        services.AddScoped<DailyReportJob>();

        services.AddSingleton<IEventBusPublisher, RabbitMqEventBusPublisher>();
        services.AddHttpClient<IWeatherService, WeatherService>();
        return services;
    }
}
using System.Threading.RateLimiting;
using FishingCommunity.API.Extensions;
using FishingCommunity.API.Middleware;
using FishingCommunity.Application;
using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Infrastructure;
using FishingCommunity.Infrastructure.BackgroundJobs;
using FishingCommunity.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


// Serilog

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});


// Services

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddRateLimitingConfiguration();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();


// Middleware pipeline

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fishing Community API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serves files from wwwroot, including wwwroot/uploads
app.UseCors("DefaultCorsPolicy");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();


// Hangfire Dashboard (Admin-only)

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthFilter() }
});

app.MapControllers();
app.MapHub<FishingCommunity.API.Hubs.ChatHub>("/hubs/chat");


// Database Seeding (Roles + Default Admin)

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        await ApplicationDbContextSeed.SeedAsync(userManager, roleManager, configuration, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// Hangfire Recurring Jobs Registration

RecurringJob.AddOrUpdate<TripReminderJob>(
    "trip-reminders",
    job => job.ExecuteAsync(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<CleanupExpiredTokensJob>(
    "cleanup-expired-tokens",
    job => job.ExecuteAsync(),
    Cron.Daily(3)); // Runs daily at 3:00 AM UTC — low-traffic window.

RecurringJob.AddOrUpdate<DailyReportJob>(
    "daily-report",
    job => job.ExecuteAsync(),
    Cron.Daily(6)); // Runs daily at 6:00 AM UTC.

app.Run();
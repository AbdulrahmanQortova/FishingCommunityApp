using FishingCommunity.Application.Common.Models;
using FishingCommunity.Infrastructure;
using FishingCommunity.NotificationWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));

// Reuses the exact same AddInfrastructure() registration as the API — this gives
// the Worker access to ApplicationDbContext, IUnitOfWork, INotificationService, and
// everything else Infrastructure provides, with zero duplicated setup code.
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<NotificationConsumerService>();

var host = builder.Build();
host.Run();
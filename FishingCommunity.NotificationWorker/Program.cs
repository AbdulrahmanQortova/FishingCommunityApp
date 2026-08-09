using FishingCommunity.Application;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Infrastructure;
using FishingCommunity.NotificationWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.SectionName));

// Both are needed: AddApplication() registers MediatR's IPublisher (required by
// AuditableEntitySaveChangesInterceptor), and AddInfrastructure() registers the
// DbContext, repositories, and everything else the Worker needs.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<NotificationConsumerService>();

var host = builder.Build();
host.Run();
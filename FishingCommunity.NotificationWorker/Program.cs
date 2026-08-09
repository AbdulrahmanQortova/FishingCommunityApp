using FishingCommunity.Application;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Infrastructure;
using FishingCommunity.NotificationWorker;

var host = Host.CreateDefaultBuilder(args)
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = false;
        options.ValidateScopes = true;
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<RabbitMqSettings>(context.Configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddApplication();
        services.AddInfrastructure(context.Configuration);
        services.AddHostedService<NotificationConsumerService>();
    })
    .Build();

host.Run();
using AgendaDemoBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("6638804507:AAHt81nlF__ePxe0MFsfvmiYCMqSX3HxpSs"));
        services.AddHostedService<TelegramBotBackgroundService>();
        services.AddHostedService<AgendaSender>();
        services.AddLogging(configure => configure.AddConsole());
    })
    .Build();

await host.RunAsync();

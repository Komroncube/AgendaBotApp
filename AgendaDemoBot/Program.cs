using AgendaDemoBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var botToken = configuration["TelegramBotToken"];
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
        services.AddHostedService<TelegramBotBackgroundService>();
        services.AddHostedService<AgendaSender>();
        services.AddLogging(configure => configure.AddConsole());
    })
    .Build();

await host.RunAsync();

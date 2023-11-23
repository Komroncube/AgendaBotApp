
using AgendaDemoBot.Data;
using AgendaDemoBot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


public class TelegramBotBackgroundService : BackgroundService
{
    private readonly Dictionary<long, bool> ReceivingAgenda;
    private readonly ILogger<TelegramBotBackgroundService> _logger;
    private readonly ITelegramBotClient _botClient;

    public TelegramBotBackgroundService(ILogger<TelegramBotBackgroundService> logger, ITelegramBotClient botClient)
    {
        _logger = logger;
        _botClient = botClient; // Replace with your bot token
        ReceivingAgenda = new Dictionary<long, bool>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram bot is starting.");

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            new ReceiverOptions
            {
                AllowedUpdates = { } // Receive all update types
            },
            cancellationToken: stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Unknown => throw new NotImplementedException(),
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.InlineQuery => throw new NotImplementedException(),
            UpdateType.ChosenInlineResult => throw new NotImplementedException(),
            UpdateType.CallbackQuery => throw new NotImplementedException(),
            UpdateType.EditedMessage => throw new NotImplementedException(),
            UpdateType.ChannelPost => throw new NotImplementedException(),
            UpdateType.EditedChannelPost => throw new NotImplementedException(),
            UpdateType.ShippingQuery => throw new NotImplementedException(),
            UpdateType.PreCheckoutQuery => throw new NotImplementedException(),
            UpdateType.Poll => throw new NotImplementedException(),
            UpdateType.PollAnswer => throw new NotImplementedException(),
            UpdateType.MyChatMember => throw new NotImplementedException(),
            UpdateType.ChatMember => throw new NotImplementedException(),
            UpdateType.ChatJoinRequest => throw new NotImplementedException()
        };
        try
        {
            await handler;
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message? updateMessage, CancellationToken cancellationToken)
    {
        if (updateMessage is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;
        long chatId = message.Chat.Id;

        string command;
        if (messageText.StartsWith(Start))
        {
            command = Start;
        }
        else if (messageText.StartsWith(TaskAgenda))
        {
            command = TaskAgenda;
        }
        else if (messageText.StartsWith(Stop))
        {
            command = Stop;
        }
        else
        {
            command = "error";
        }
        var commandHandler = command switch
        {
            Start => StartCommandAsync(botClient, message, cancellationToken),
            TaskAgenda => TaskAgendaCommand(botClient, message, cancellationToken),
            Stop => StopCommandAsync(botClient, message, cancellationToken)
        };
        try
        {
            await commandHandler;
        }
        catch { await Console.Out.WriteLineAsync(command); }

    }

    private async ValueTask StartCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        User user = message.From;
        
        var users = await AgendaTaskDataAccess.GetAllUsers();
       
        if(!users.Any(x=>x.UserId == user.Id))
        {
            BotUser botUser = new BotUser
            {
                UserId = user.Id,
                UserName = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            await AgendaTaskDataAccess.CreateUser(botUser);

            await botClient.SendTextMessageAsync(
                chatId: user.Id,
                text: $"Assalomu alaykum {user.Username}\n" +
                $"/task so'zidan keyin topshiriqni yozing",
                cancellationToken: cancellationToken
                );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: user.Id,
                text: "Siz ro'yxatdan o'tgansiz",
                cancellationToken: cancellationToken);
        }
        
    }

    private async ValueTask StopCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        BotUser botUser = await AgendaTaskDataAccess.GetUserByIdAsync(message.From.Id);
        botUser.IsActive = false;
        await AgendaTaskDataAccess.UpdateUser(botUser);
    }

    private async ValueTask TaskAgendaCommand(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        BotUser user = await AgendaTaskDataAccess.GetUserByIdAsync(message.From.Id);
        var task = new AgendaTask()
        {
            TaskDefinition = message.Text,
        };
        user.Tasks.Add(task);
        await AgendaTaskDataAccess.UpdateUser(user);
    }
    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred in Telegram bot.");
        return Task.CompletedTask;
    }
    private const string Start = "/start";
    private const string TaskAgenda = "/task";
    private const string Stop = "/stop";

}

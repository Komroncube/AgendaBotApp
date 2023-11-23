using AgendaDemoBot.Data;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace AgendaDemoBot
{
    public class AgendaSender : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;

        public AgendaSender(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                if (now.Hour == 17 && now.Minute == 35 && now.Second == 0)
                {
                    var users = await AgendaTaskDataAccess.GetAllUsers();
                    foreach (var user in users.Where(x => x.IsActive == true))
                    {
                        foreach (var task in user.Tasks)
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: user.UserId,
                                text: task.TaskDefinition,
                                cancellationToken: stoppingToken);
                            task.LastSendDate = now;
                        }
                        await AgendaTaskDataAccess.UpdateUser(user);
                    }
                }
            }
        }

    }
}

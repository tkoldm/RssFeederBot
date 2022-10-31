using Domain.Services.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Infrastructure.Bot;

public class Bot
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserStore _userStore;

    private const string CommandStart = "/start";
    private const string CommandAdd = "/add ";

    public Bot(string apiKey, IUserStore userStore)
    {
        _userStore = userStore;
        _bot = new TelegramBotClient(apiKey);
    }
    public void StartReceiving(
        ReceiverOptions? receiverOptions = null,
        CancellationToken cancellationToken = default)
    {
        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            if (message?.Text == null)
            {
                return;
            }

            if (message.Text.ToLower() == CommandStart)
            {
                await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!",
                    cancellationToken: cancellationToken);
            }
            
            if (message.Text.ToLower().StartsWith(CommandAdd))
            {
                var link = message.Text.Split(CommandAdd)[1];
                var id = message.Chat.Id;
                if (await _userStore.IsDataExist(id))
                {
                    await _userStore.AddUserLink(id, link);
                }
                else
                {
                    await _userStore.AddUser(id, new List<string>() { link });
                }
                await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!",
                    cancellationToken: cancellationToken);
            }
            
        }
    }

    public async Task<bool> SendMessage(string message, Chat chat)
    {
        try
        {
            await _bot.SendTextMessageAsync(chat, message);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
}
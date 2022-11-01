using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Infrastructure.Bot;

public class Bot
{
    private readonly ITelegramBotClient _bot;
    private readonly LinksStorage _linksStorage;

    private const string CommandStart = "/start";
    private const string CommandAdd = "/add ";

    public Bot(string apiKey, LinksStorage linksStorage)
    {
        _linksStorage = linksStorage;
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
                _linksStorage.AddLink(link, id);
            }
            
        }
    }

    public async Task SendMessage(string message, long[] chatIds)
    {
        foreach (var chatId in chatIds)
        {
            try
            {
                await _bot.SendTextMessageAsync(new Chat{Id = chatId}, message);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
}
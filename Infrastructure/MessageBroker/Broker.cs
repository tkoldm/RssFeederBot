using Domain.Models;
using Domain.Services.Abstractions;
using Telegram.Bot.Types;

namespace Infrastructure.MessageBroker;

public class Broker : IDisposable
{
    private readonly List<ParsedModel> _queuedModels = new();
    private readonly IUserStore _users;
    private readonly Bot.Bot _bot;
    private readonly IParser _rssParser;
    private readonly Thread _parserWorker;
    private readonly Thread _botWorker;
    private readonly object _locker = new();
    private readonly Semaphore _semaphoreBot = new(1, 1);
    private readonly Semaphore _semaphoreParser = new(1, 1);

    public Broker(Bot.Bot bot, IParser parser, IUserStore users)
    {
        _bot = bot;
        _rssParser = parser;
        _users = users;
        _botWorker = new Thread(Consume);
        _botWorker.Start();
        _parserWorker = new Thread(Produce);
        _parserWorker.Start();
    }

    /// <summary>
    /// Add message to queue from parser
    /// </summary>
    private async void Produce()
    {
        while (true)
        {
            if (!await _users.Any())
            {
                Thread.Sleep(600);
                continue;
            }

            var ids = await _users.GetChatIds();
            foreach (var user in ids)
            {
                _semaphoreParser.WaitOne();
                var rssUrls = await _users.GetLinksByChatId(user);
                var models = await _rssParser.ParseAsync(rssUrls, user);
                _queuedModels.AddRange(models);
                _semaphoreParser.Release();
            }
            Thread.Sleep(TimeSpan.FromSeconds(400));
        }
    }

    /// <summary>
    /// Process message from queue
    /// </summary>
    /// <returns></returns>
    private async void Consume()
    {
        while (true)
        {
            ParsedModel? message;
            lock (_locker)
            {
                message = _queuedModels.FirstOrDefault();
            }

            if (message == null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(600));
                continue;
            }

            _semaphoreBot.WaitOne();
            await _bot.SendMessage(message.ToString(), new Chat { Id = message.ChatId });
            _queuedModels.Remove(message);
            _semaphoreBot.Release();
            Thread.Sleep(TimeSpan.FromSeconds(600));
        }
    }

    public void Dispose()
    {
        try
        {
            _parserWorker.Interrupt();
            _botWorker.Interrupt();
        }
        finally
        {
            _semaphoreBot.Close();
            _semaphoreParser.Close();
        }
    }
}
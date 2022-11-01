using Domain.Models;

namespace Infrastructure.MessageBroker;

public class BotBroker : IDisposable
{
    private readonly Queue<ParsedModel> _queuedModels = new();
    private readonly Bot.Bot _bot;
    private readonly Thread _botWorker;
    private readonly object _locker = new();
    private readonly Semaphore _semaphoreBot = new(1, 1);

    public BotBroker(Bot.Bot bot)
    {
        _bot = bot;
        _botWorker = new Thread(Consume);
        _botWorker.Start();
    }

    /// <summary>
    /// Add message to queue from parser
    /// </summary>
    public void Produce(ParsedModel model)
    {
        lock (_locker)
        {
            _queuedModels.Enqueue(model);
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
            bool isDequeued;
            lock (_locker)
            {
                isDequeued = _queuedModels.TryDequeue(out message);
            }

            if (!isDequeued)
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
                continue;
            }

            if (message == null)
            {
                return;
            }

            _semaphoreBot.WaitOne();
            await _bot.SendMessage(message.ToString(), message.ChatIds);
            _semaphoreBot.Release();
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }

    public void Dispose()
    {
        Produce(null);
        _botWorker.Join();
        _semaphoreBot.Close();
    }
}
using Domain.Models;

namespace Infrastructure.MessageBroker;

public class ParserBroker : IDisposable
{
    private readonly Thread _worker;
    private readonly object _locker = new();
    private readonly Queue<ParsedModel> _models = new();
    private readonly BotBroker _botSender;
    private readonly Semaphore _semaphore = new(1, 1);
    
    public ParserBroker(BotBroker botSender)
    {
        _worker = new Thread(Comsume);
        _botSender = botSender;
        _worker.Start();
    }

    public void Produce(List<ParsedModel> model)
    {
        lock (_locker)
        {
            model.ForEach(m => _models.Enqueue(m));
        }
    }

    private void Comsume()
    {
        while (true)
        {
            ParsedModel message;
            bool isDequeued;
            lock (_locker)
            {
                isDequeued = _models.TryDequeue(out message);
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

            _semaphore.WaitOne();
            _botSender.Produce(message);
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        Produce(null);
        _worker.Join();
        _botSender.Dispose();
        _semaphore.Close();
    }
}
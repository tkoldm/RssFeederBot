using Domain.Models;

namespace Infrastructure.MessageBroker;

public class Broker
{
    private List<ParsedModel> _queuedModels = new ();

    /// <summary>
    /// Добавить одиночное сообщение в очередь
    /// </summary>
    /// <param name="model"></param>
    public void Produce(ParsedModel model)
    {
        _queuedModels.Add(model);
    }
    
    /// <summary>
    /// Добавить несколько сообщений в очередь
    /// </summary>
    /// <param name="models"></param>
    internal void Produce(IEnumerable<ParsedModel> models)
    {
        _queuedModels.AddRange(models);
    }

    /// <summary>
    /// Проверка есть ли сообщения в очереди
    /// </summary>
    /// <returns></returns>
    internal bool IsMessages()
    {
        return _queuedModels.Count > 0;
    }

    /// <summary>
    /// Получить сообщение из очереди
    /// </summary>
    /// <returns></returns>
    internal ParsedModel Consume()
    {
        return _queuedModels.First();
    }

    /// <summary>
    /// Удалить сообщение из очереди
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    internal bool ConfirmConsume(ParsedModel model)
    {
        var isRemoved = _queuedModels.Remove(model);
        return isRemoved;
    }
}
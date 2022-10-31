using Domain.Models;

namespace Domain.Services.Abstractions;

public interface IParser
{
    /// <summary>
    /// Parse resource on link
    /// </summary>
    /// <param name="urls">Collection of links to resources</param>
    /// <param name="chatId">Identifier of chat with user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of articles models (title, description, link)</returns>
    Task<IEnumerable<ParsedModel>> ParseAsync(IEnumerable<string> urls, long chatId, CancellationToken cancellationToken = default);
}
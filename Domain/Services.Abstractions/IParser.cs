namespace Domain.Services.Abstractions;

public interface IParser
{
    /// <summary>
    /// Parse resource on link
    /// </summary>
    /// <param name="urls">Collection of links to resources</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of articles models (title, description, link)</returns>
    Task ParseAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default);
}
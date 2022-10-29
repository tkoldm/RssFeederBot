namespace Domain.Services.Abstractions;

public interface IUserStore
{
    /// <summary>
    /// Adding new key-value pair
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <param name="rssLinks" example="['https://rss1.com']">Links that need be added</param>
    /// <returns>Flag of result</returns>
    Task<bool> AddUser(int chatId, List<string> rssLinks);

    /// <summary>
    /// Checks is exists record for current user
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <returns>Flag of result</returns>
    Task<bool> IsDataExist(int chatId);

    /// <summary>
    /// Get the links by chatId
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <returns>Links for current chat</returns>
    Task<List<string>> GetLinksByChatId(int chatId);
}
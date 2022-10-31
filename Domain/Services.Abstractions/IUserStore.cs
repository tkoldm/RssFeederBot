namespace Domain.Services.Abstractions;

public interface IUserStore
{
    /// <summary>
    /// Adding new key-value pair
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <param name="rssLinks" example="['https://rss1.com']">Links that need be added</param>
    /// <returns>Flag of result</returns>
    Task<bool> AddUser(long chatId, List<string> rssLinks);
    
    /// <summary>
    /// Adding new link to existing user
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <param name="rssLinks" example="['https://rss1.com']">Links that need be added</param>
    /// <returns>Flag of result</returns>
    Task<bool> AddUserLink(long chatId, string rssLinks);

    /// <summary>
    /// Checks is exists record for current user
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <returns>Flag of result</returns>
    Task<bool> IsDataExist(long chatId);

    /// <summary>
    /// Check is any user were added into store
    /// </summary>
    /// <returns>Flag of existing</returns>
    Task<bool> Any();

    /// <summary>
    /// Get the links by chatId
    /// </summary>
    /// <param name="chatId" example="9845245">Chat identifier from request</param>
    /// <returns>Links for current chat</returns>
    Task<string[]> GetLinksByChatId(long chatId);

    /// <summary>
    /// Get all chats ids from user store
    /// </summary>
    /// <returns>Array of chat's identifiers</returns>
    Task<long[]> GetChatIds();
}
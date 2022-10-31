using Domain.Services.Abstractions;

namespace Infrastructure.UserStore;

public class UserStore : IUserStore
{
    private IDictionary<long, List<string>> _userStore;

    public UserStore()
    {
        _userStore = new Dictionary<long, List<string>>();
    }

    public Task<bool> AddUser(long chatId, List<string> rssLinks)
    {
        if (_userStore.ContainsKey(chatId)) return Task.FromResult(false);
        var isAdded = _userStore.TryAdd(chatId, rssLinks);
        return Task.FromResult(isAdded);
    }

    public Task<bool> AddUserLink(long chatId, string rssLink)
    {
        _userStore[chatId].Add(rssLink);
        return Task.FromResult(true);
    }

    public Task<bool> Any() => Task.FromResult(_userStore.Any());

    public Task<bool> IsDataExist(long chatId)
    {
        var isExist = _userStore.ContainsKey(chatId);
        return Task.FromResult(isExist);
    }

    public Task<string[]> GetLinksByChatId(long chatId)
    {
        if (_userStore.ContainsKey(chatId))
        {
            return Task.FromResult(_userStore[chatId].ToArray());
        }

        throw new ArgumentOutOfRangeException(nameof(chatId), "User not found");
    }

    public Task<long[]> GetChatIds()
    {
        return Task.FromResult(_userStore.Keys.ToArray());
    }
}
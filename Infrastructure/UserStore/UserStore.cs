using Domain.Services.Abstractions;

namespace Infrastructure.UserStore;

public class UserStore : IUserStore
{
    private IDictionary<int, List<string>> _userStore;

    public UserStore()
    {
        _userStore = new Dictionary<int, List<string>>();
    }

    public Task<bool> AddUser(int chatId, List<string> rssLinks)
    {
        if (_userStore.ContainsKey(chatId)) return Task.FromResult(false);
        var isAdded = _userStore.TryAdd(chatId, rssLinks);
        return Task.FromResult(isAdded);
    }

    public Task<bool> IsDataExist(int chatId)
    {
        var isExist = _userStore.ContainsKey(chatId);
        return Task.FromResult(isExist);
    }

    public Task<List<string>> GetLinksByChatId(int chatId)
    {
        if (_userStore.ContainsKey(chatId))
        {
            return Task.FromResult(_userStore[chatId]);
        }

        throw new ArgumentOutOfRangeException(nameof(chatId), "User not found");
    }
}
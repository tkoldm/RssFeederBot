namespace Infrastructure;

public class LinksStorage
{
    private readonly IDictionary<string, List<long>> _linkUsers;

    public LinksStorage()
    {
        _linkUsers = new Dictionary<string, List<long>>();
    }

    public void AddLink(string link, long chatId)
    {
        var isLinkAdded = _linkUsers.ContainsKey(link);
        if (isLinkAdded)
        {
            _linkUsers[link].Add(chatId);
        }
        else
        {
            _linkUsers.Add(link, new List<long>{chatId});
        }
    }

    public string[] GetLinks() => _linkUsers.Keys.ToArray();
    public long[] GetUsersByLink(string link)
    {
        var isExist = _linkUsers.TryGetValue(link, out var chatIds);
        return isExist ? chatIds.ToArray() : Array.Empty<long>();
    }
}
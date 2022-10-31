namespace Domain.Models;

public class ParsedModel
{
    /// <summary>
    /// Article's identifier
    /// </summary>
    public Guid Identifier { get; }
    
    /// <summary>
    /// Identifier of chat
    /// </summary>
    public long ChatId { get; private set; }

    /// <summary>
    /// Article's title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Article's description
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Url to original resource
    /// </summary>
    public string OriginalLink { get; }

    /// <summary>
    /// Data of article from external resource
    /// </summary>
    /// <param name="title">Title of article</param>
    /// <param name="description">Description</param>
    /// <param name="originalLink">Url to original resource</param>
    /// <param name="chatId">Identifier of chat with user</param>
    public ParsedModel(string title, string description, string originalLink, long chatId)
    {
        Identifier = Guid.NewGuid();
        Title = title;
        Description = description;
        OriginalLink = originalLink;
        ChatId = chatId;
    }

    public override string ToString()
    {
        return $"{Title}\n{Description.Substring(0, Math.Min(Description.Length - 1, 2000))}\n{OriginalLink}\n\n";
    }
}